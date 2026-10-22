namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    internal static class DocumentDiff
    {
        public static UpdateBuffer<TDomain> Compare<TDomain>(TDomain source, TDomain target)
        {
            var buffer = new UpdateBuffer<TDomain>();
            if (!DiffCompiler<TDomain>.TryApply(source, target, buffer))
            {
                throw new ClassicDomainException(typeof(TDomain), "比较器生成失败。");
            }
            return buffer;
        }
    }

    internal static class DiffCompiler<TDomain>
    {
        private static readonly System.Action<TDomain, TDomain, string, IUpdateBuffer> Action;

        private static readonly bool Failed;

        static DiffCompiler()
        {
            try
            {
                Action = (System.Action<TDomain, TDomain, string, IUpdateBuffer>)DiffEmitter.Compile(typeof(TDomain));
            }
            catch (System.Exception ex)
            {
                Failed = true;
                System.Diagnostics.Trace.TraceWarning("比较器生成失败，回退反射比较：{0}。{1}", typeof(TDomain).FullName, ex.Message);
            }
        }

        public static bool TryApply(TDomain source, TDomain target, IUpdateBuffer buffer)
        {
            if (Failed) return false;
            Action(source, target, string.Empty, buffer);
            return true;
        }
    }
}
