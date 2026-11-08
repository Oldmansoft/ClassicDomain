using System;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// 让动态程序集访问目标程序集的非公开成员。运行时按这个类型全名识别。
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    sealed class IgnoresAccessChecksToAttribute : Attribute
    {
        public IgnoresAccessChecksToAttribute(string assemblyName)
        {
            AssemblyName = assemblyName;
        }

        public string AssemblyName { get; }
    }
}
