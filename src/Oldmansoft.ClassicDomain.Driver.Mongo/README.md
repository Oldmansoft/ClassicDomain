# Oldmansoft.ClassicDomain.Driver.Mongo

`Oldmansoft.ClassicDomain` 的 MongoDB 驱动。用仓储和工作单元访问 MongoDB：在上下文里登记实体，查询走 LINQ，`Commit` 时批量写入。

目标框架 `netstandard2.0`，依赖 `MongoDB.Driver` 2.30.0。

```
dotnet add package Oldmansoft.ClassicDomain.Driver.Mongo
```

## 连接

在第一次创建上下文之前设置连接字符串。数据库名取路径部分，路径为空时使用 `DefaultDatabase`。

```csharp
using Oldmansoft.ClassicDomain;

ConnectionString.Set<Mapping>("mongodb://localhost/Sample");
```

格式：`mongodb://[username:password@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[database][?options]]`。详见 [MongoDB 连接字符串](https://www.mongodb.com/docs/manual/reference/connection-string/)。

每个上下文类型会缓存一个 `MongoClient`。连接字符串需要在该上下文第一次使用前设好。

## 映射

继承 `Oldmansoft.ClassicDomain.Driver.Mongo.Context`，在 `OnModelCreating` 里登记实体。主键属性名必须是 `Id`，文档里写成 `_id`。集合名默认是类名，可用 `ToTable` 修改。

```csharp
using Oldmansoft.ClassicDomain.Driver.Mongo;

public class Mapping : Context
{
    protected override void OnModelCreating()
    {
        Add<Person, Guid>(o => o.Id)
            .ToTable("people")
            .SetUnique(o => o.CreateGroup(x => x.LastName).Add(x => x.FirstName));
    }
}
```

索引在登记时创建（已存在则跳过）：

| 方法 | 作用 |
| --- | --- |
| `SetIndex` / `SetIndexDescending` | 升序或降序索引，可组合多字段 |
| `SetUnique` / `SetUniqueDescending` | 唯一索引 |
| `SetGeoSpatial` | 2d 索引 |
| `SetGeoSpatialSpherical` | 2dsphere 索引 |

主键为 `Guid` 且值为 `Guid.Empty` 时，`Add` 会自动赋值。`Guid` 按标准 UUID 存储。

## 仓储

```csharp
using System.Linq;
using Oldmansoft.ClassicDomain;
using Oldmansoft.ClassicDomain.Driver.Mongo;

public interface IPersonRepository : IRepository<Person, Guid>
{
    Person GetByName(string lastName, string firstName);
}

public class PersonRepository : Repository<Person, Guid, Mapping>, IPersonRepository
{
    public Person GetByName(string lastName, string firstName)
    {
        return Query().FirstOrDefault(o => o.LastName == lastName && o.FirstName == firstName);
    }
}

public class PersonFactory : RepositoryFactory
{
    static PersonFactory()
    {
        Add<IPersonRepository, PersonRepository>();
    }
}
```

实体只要有名为 `Id` 的主键。下面是一个最小领域类：

```csharp
public class Person
{
    public Guid Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    private Person() { }

    public static Person Create(string firstName, string lastName)
    {
        return new Person { FirstName = firstName, LastName = lastName };
    }

    public void Change(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
```

## 写入

`Add`、`Replace`、`Remove` 只登记变更，调用 `Commit` 后才写入。同一上下文里的写入会合并成一次 `BulkWrite`。

```csharp
using (var factory = new PersonFactory())
{
    var repository = factory.GetRepository<IPersonRepository>();
    repository.Add(Person.Create("Ming", "Li"));
    factory.GetUnitOfWork().Commit();
}
```

更新和删除：

```csharp
using (var factory = new PersonFactory())
{
    var repository = factory.GetRepository<IPersonRepository>();
    var person = repository.GetByName("Li", "Ming");
    person.Change("Hua", "Li");
    repository.Replace(person);
    factory.GetUnitOfWork().Commit();
}
```

`Replace` 的行为：

- 实体是当前工作单元里 `Get` 或 `Query` 加载出来的：按标识映射做差量更新，只写变化的字段。
- 实体不是这次加载的：整文档 `ReplaceOne`。

唯一索引冲突（MongoDB 错误码 11000）抛出 `UniqueException`。

分页从 1 开始：

```csharp
var page = repository.Query()
    .Paging()
    .OrderBy(o => o.LastName)
    .ThenBy(o => o.FirstName)
    .Size(20)
    .ToList(1, out int totalCount);
```

## 直接访问集合

`Execute` 把操作登记到本次 `Commit`。带返回值的 `Execute` 立刻执行。

```csharp
repository.Execute(collection =>
{
    collection.InsertMany(people);
    return true;
});
factory.GetUnitOfWork().Commit();
```

子类实体要按父类仓储接口暴露时，继承 `RepositoryDefinedSuperClass<TDomain, TSuperDomain, TKey, TContext>`。
