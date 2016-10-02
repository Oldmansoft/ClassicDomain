using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oldmansoft.ClassicDomain.Util;

namespace Oldmansoft.ClassicDomain.Driver.Mongo
{
    /// <summary>
    /// 快速模式实体上下文
    /// 数据库连接串格式 mongodb://[username:password@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[database][?options]]
    /// 更多请参考 https://docs.mongodb.com/manual/reference/connection-string/
    /// </summary>
    public abstract class FastModeContext : Core.FastModeContext, IContext
    {   
    }

    /// <summary>
    /// 可传入初始化参数的快速模式实体上下文
    /// </summary>
    /// <typeparam name="TInit">初始化参数类型</typeparam>
    public abstract class FastModeContext<TInit> : Core.FastModeContext, IContext<TInit>
    {
        /// <summary>
        /// 创建实体中，此方法由 UnitOfWork 调用
        /// </summary>
        /// <param name="parameter">初始化参数</param>
        void IUnitOfWorkManagedItem<TInit>.ModelCreating(TInit parameter)
        {
            OnModelCreating(parameter);
        }

        /// <summary>
        /// 创建实体中
        /// </summary>
        protected override void OnModelCreating()
        {
        }

        /// <summary>
        /// 创建实体中
        /// </summary>
        /// <param name="parameter">初始化参数</param>
        protected abstract void OnModelCreating(TInit parameter);
    }
}
