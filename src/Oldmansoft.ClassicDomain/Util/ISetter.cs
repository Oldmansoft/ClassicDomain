namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 设值器
    /// </summary>
    public interface ISetter : IContent
    {
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="value"></param>
        void Set(object caller, object value);
    }
}
