using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    class CursorProxy<TResult> : IAsyncCursor<TResult>
    {
        private readonly IAsyncCursor<TResult> Source;

        public CursorProxy(IAsyncCursor<TResult> source)
        {
            Source = source;
        }

        IEnumerable<TResult> IAsyncCursor<TResult>.Current => new Enumerable<TResult>(Source.Current);

        void IDisposable.Dispose()
        {
            Source.Dispose();
        }

        bool IAsyncCursor<TResult>.MoveNext(CancellationToken cancellationToken)
        {
            return Source.MoveNext(cancellationToken);
        }

        Task<bool> IAsyncCursor<TResult>.MoveNextAsync(CancellationToken cancellationToken)
        {
            return Source.MoveNextAsync(cancellationToken);
        }
    }

    class Enumerable<TResult> : IEnumerable<TResult>
    {
        private readonly IEnumerable<TResult> Source;

        public Enumerable(IEnumerable<TResult> source)
        {
            Source = source;
        }

        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator()
        {
            return new Enumerator<TResult>(Source.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Source).GetEnumerator();
        }
    }

    class Enumerator<TResult> : IEnumerator<TResult>
    {
        private readonly IEnumerator<TResult> Source;

        public Enumerator(IEnumerator<TResult> source)
        {
            Source = source;
        }

        TResult IEnumerator<TResult>.Current
        {
            get
            {
                return Source.Current;
            }
        }

        object IEnumerator.Current => ((IEnumerator)Source).Current;

        void IDisposable.Dispose()
        {
            Source.Dispose();
        }

        bool IEnumerator.MoveNext()
        {
            return Source.MoveNext();
        }

        void IEnumerator.Reset()
        {
            Source.Reset();
        }
    }
}
