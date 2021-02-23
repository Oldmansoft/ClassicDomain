namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 获取
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IGet<TDomain, TKey> where TDomain : class
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TDomain Get(TKey id);
    }
}