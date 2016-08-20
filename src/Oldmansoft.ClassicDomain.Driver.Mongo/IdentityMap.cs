using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 标识映射
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal class IdentityMap<TEntity>
    {
        private Dictionary<string, TEntity> Store;

        private Util.DataMapper Mapper;

        /// <summary>
        /// 获取主键
        /// </summary>
        public Func<TEntity, object> GetKey { get; private set; }

        /// <summary>
        /// 创建标识映射
        /// </summary>
        public IdentityMap()
        {
            Store = new Dictionary<string, TEntity>();
            Mapper = new Util.DataMapper(true);
        }

        /// <summary>
        /// 设置主键
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="keyExpression"></param>
        public void SetKey<TMember>(Func<TEntity, TMember> keyExpression)
        {
            GetKey = (entity) => { return keyExpression(entity); };
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="entity"></param>
        public void Set(TEntity entity)
        {
            var domain = Activator.CreateInstance<TEntity>();
            Store[GetKey(entity).ToString()] = Mapper.CopyTo(entity, domain);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TEntity Get(object key)
        {
            var index = key.ToString();
            if (Store.ContainsKey(index))
            {
                return Store[index];
            }
            else
            {
                return default(TEntity);
            }
        }

        /// <summary>
        /// 移除值
        /// </summary>
        /// <param name="key"></param>
        public void Remove(object key)
        {
            Store.Remove(key.ToString());
        }
    }
}
