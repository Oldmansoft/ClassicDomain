namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 获值器
    /// </summary>
    public interface IGetter : IContent
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        object Get(object caller);
    }
}
