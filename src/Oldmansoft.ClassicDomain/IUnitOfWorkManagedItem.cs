namespace Oldmansoft.ClassicDomain
{
    /// <summary>
    /// 工作单元管理项
    /// </summary>
    public interface IUnitOfWorkManagedItem
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="commands"></param>
        void Init(System.Collections.Concurrent.ConcurrentQueue<Driver.ICommand> commands);

        /// <summary>
        /// 创建实体中
        /// </summary>
        void ModelCreating();
    }
}
