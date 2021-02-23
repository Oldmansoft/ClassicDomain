using System;

namespace Oldmansoft.ClassicDomain.Util
{
    /// <summary>
    /// 内容
    /// </summary>
    public interface IContent
    {
        /// <summary>
        /// 类型
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
    }
}
