using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain.Util;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    /// <summary>
    /// 实体上下文
    /// </summary>
    public abstract class Context : UnitOfWorkManagedItem, IContext
    {
        private Dictionary<Type, IDbSet> DbSet { get; set; }
        
        /// <summary>
        /// 创建 Mongo 的实体上下文
        /// </summary>
        public Context()
        {
            DbSet = new Dictionary<Type, IDbSet>();
        }

        /// <summary>
        /// 创建实体集
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        internal abstract IDbSet<TDomain, TKey> CreateDbSet<TDomain, TKey>(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression);

        /// <summary>
        /// 添加领域上下文
        /// 主键表达式必须为 Id
        /// </summary>
        /// <typeparam name="TDomain">实体类型</typeparam>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="keyExpression">主键表达式</param>
        /// <returns>设置</returns>
        public Setting<TDomain, TKey> Add<TDomain, TKey>(System.Linq.Expressions.Expression<Func<TDomain, TKey>> keyExpression)
        {
            Type type = typeof(TDomain);
            if (DbSet.ContainsKey(type))
            {
                throw new ArgumentException("已添加了此实体类型。");
            }
            if (keyExpression == null)
            {
                throw new ArgumentNullException("keyExpression");
            }
            if (keyExpression.GetProperty().Name != "Id")
            {
                throw new ArgumentException("主键表达式必须为 Id");
            }
            var dbSet = CreateDbSet(keyExpression);
            DbSet.Add(type, dbSet);
            return new Setting<TDomain, TKey>(dbSet);
        }

        IDbSet<TDomain, TKey> IContext.Set<TDomain, TKey>()
        {
            Type type = typeof(TDomain);
            if (!DbSet.ContainsKey(type))
            {
                throw new ClassicDomainException(string.Format("{0} 类型没有添加到 {1} 上下文中。", type.FullName, this.GetType().FullName));
            }
            var result = DbSet[type] as IDbSet<TDomain, TKey>;
            if (result == null)
            {
                throw new ClassicDomainException("Set 获取的主键类型与 Add 添加的主键类型不一致。");
            }
            return result;
        }
        
        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public override int Commit()
        {
            try
            {
                int result = 0;
                foreach (IDbSet item in DbSet.Values)
                {
                    result += item.Commit();
                }
                return result;
            }
            catch (MongoDB.Driver.MongoDuplicateKeyException ex)
            {
                throw new UniqueException(ex);
            }
        }
    }
}
