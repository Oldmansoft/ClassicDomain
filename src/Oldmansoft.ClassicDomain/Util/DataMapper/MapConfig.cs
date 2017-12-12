using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 映射配置
    /// </summary>
    public class MapConfig
    {
        /// <summary>
        /// 忽略属性
        /// </summary>
        internal HashSet<string> IgnoreProperty { get; set; }

        /// <summary>
        /// 深拷贝
        /// </summary>
        public bool DeepCopy { get; set; }

        /// <summary>
        /// 忽略源空值
        /// </summary>
        public bool IgnoreSourceNull { get; set; }
        
        /// <summary>
        /// 设置忽略属性的实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IPropertyIgnore<TEntity> SetIgnore<TEntity>()
        {
            return new PropertyIgnore<TEntity>(IgnoreProperty);
        }

        /// <summary>
        /// 创建配置
        /// </summary>
        public MapConfig()
        {
            DeepCopy = true;
            IgnoreSourceNull = false;
            IgnoreProperty = new HashSet<string>();
        }
    }
}
