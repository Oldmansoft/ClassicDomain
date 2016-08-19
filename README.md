ClassicDomain C# Driver
=================

You can get the latest stable release from the [official Nuget.org feed](https://www.nuget.org/packages/Domain.Net).


Getting Started
---------------

### Typed Documents

```C#
using Oldmansoft.ClassicDomain;
using Oldmansoft.ClassicDomain.Driver.Mongo;
```

```C#
public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
```

```C#
class Mapping : Context
{
    public override void OnModelCreating()
    {
        Add<Person, Guid>(o => o.Id).SetUnique(o => o.Name);
    }
}
```

```C#
public class RepositoryFactory
{
    private UnitOfWork Uow { get; set; }

     public RepositoryFactory()
    {
        Uow = new UnitOfWork();
    }

    public IUnitOfWork GetUnitOfWork()
    {
        return Uow;
    }

    public IRepository<Person, Guid> CreatePerson()
    {
        return new Repository<Person, Guid, Mapping>(Uow);
    }
}
```

### Maintainers:
* Oldman                    https://github.com/Oldmansoft

If you have contributed and we have neglected to add you to this list please contact one of the maintainers to be added to the list (with apologies).
