namespace Oldmansoft.ClassicDomain.Driver
{
    /// <summary>
    /// 提交结束时再统一落库
    /// </summary>
    public interface IUnitOfWorkFlushable
    {
        /// <summary>
        /// 把已经挂上的变更一次写入，返回受影响的数量
        /// </summary>
        /// <returns></returns>
        int Flush();
    }
}
