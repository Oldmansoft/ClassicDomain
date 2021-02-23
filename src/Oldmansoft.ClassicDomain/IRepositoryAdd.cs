namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 获取，查询，添加
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepositoryAdd<TDomain, TKey> :
        IGet<TDomain, TKey>,
        IAdd<TDomain>,
        IRepository
        where TDomain : class
    {
    }
}
