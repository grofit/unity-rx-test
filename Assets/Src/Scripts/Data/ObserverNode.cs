using System.Threading;

namespace System.Reactive.Data {
    internal sealed class ObserverNode<T> : IObserver<T>, IDisposable
    {
        readonly IObserver<T> observer;
        IObserverLinkedList<T> list;

        public ObserverNode<T> Previous { get; internal set; }
        public ObserverNode<T> Next { get; internal set; }

        public ObserverNode(IObserverLinkedList<T> list, IObserver<T> observer)
        {
            this.list = list;
            this.observer = observer;
        }

        public void OnNext(T value)
        {
            observer.OnNext(value);
        }

        public void OnError(Exception error)
        {
            observer.OnError(error);
        }

        public void OnCompleted()
        {
            observer.OnCompleted();
        }

        public void Dispose()
        {
            var sourceList = Interlocked.Exchange(ref list, null);
            if (sourceList != null)
            {
                sourceList.UnsubscribeNode(this);
                sourceList = null;
            }
        }
    }
}
