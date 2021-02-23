namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 仓储
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// 设置工作单元
        /// </summary>
        /// <param name="uow"></param>
        void SetUnitOfWork(UnitOfWork uow);
    }

    /// <summary>
    /// 全部
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<TDomain, TKey> :
        IAdd<TDomain>,
        IReplace<TDomain>,
        IRemove<TDomain>,
        IGet<TDomain, TKey>,
        IRepositoryAdd<TDomain, TKey>,
        IRepositoryAddReplace<TDomain, TKey>
        where TDomain : class
    {
    }
}
