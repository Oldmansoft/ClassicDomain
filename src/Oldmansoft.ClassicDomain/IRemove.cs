namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 移除
    /// </summary>
    public interface IRemove<TDomain> where TDomain : class
    {
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="domain"></param>
        void Remove(TDomain domain);
    }
}
