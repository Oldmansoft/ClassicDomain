namespace Oldmansoft.ClassicDomain.Driver
{
    /// <summary>
    /// 命令
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        bool Execute();
    }
}
