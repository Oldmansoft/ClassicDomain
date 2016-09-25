using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis
{
    /// <summary>
    /// 实体上下文接口
    /// </summary>
    public interface IContext : Core.IContext
    {
    }

    /// <summary>
    /// 可传入初始化参数的实体上下文接口
    /// </summary>
    /// <typeparam name="TInit">初始化参数类型</typeparam>
    public interface IContext<TInit> : Core.IContext, IUnitOfWorkManagedItem<TInit>
    {
    }
}
