using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    internal class MongoCursorEnumerator<TDocument> : IEnumerator<TDocument>
    {
        private IEnumerator<TDocument> Source;

        private Core.IdentityMap<TDocument> IdentityMap;

        public MongoCursorEnumerator(IEnumerator<TDocument> source, Core.IdentityMap<TDocument> identityMap)
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
