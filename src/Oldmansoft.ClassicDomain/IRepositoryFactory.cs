using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 仓储工厂
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// 获取工作单元
        /// </summary>
        /// <returns></returns>
        UnitOfWork GetUnitOfWork();

        /// <summary>
        /// 获取仓储
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        TRepository GetRepository<TRepository>() where TRepository : IRepository;
    }
}
