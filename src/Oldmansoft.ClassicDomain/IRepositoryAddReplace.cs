namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 获取，查询，添加，替换
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepositoryAddReplace<TDomain, TKey> :
        IGet<TDomain, TKey>,
        IAdd<TDomain>,
        IReplace<TDomain>,
        IRepositoryAdd<TDomain, TKey>
        where TDomain : class
    {
    }
}
