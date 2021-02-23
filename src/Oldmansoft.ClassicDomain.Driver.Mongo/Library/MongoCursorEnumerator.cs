using System.Collections.Generic;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    internal class MongoCursorEnumerator<TDocument> : IEnumerator<TDocument>
    {
        private readonly IEnumerator<TDocument> Source;

        private readonly IdentityMap<TDocument> IdentityMap;

        public MongoCursorEnumerator(IEnumerator<TDocument> source, IdentityMap<TDocument> identityMap)
        {
            Source = source;
            IdentityMap = identityMap;
        }

        public TDocument Current
        {
            get
            {
                IdentityMap.Set(Source.Current);
                return Source.Current;
            }
        }

        public void Dispose()
        {
            Source.Dispose();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Source.Current; }
        }

        public bool MoveNext()
        {
            return Source.MoveNext();
        }

        public void Reset()
        {
            Source.Reset();
        }
    }
}
