using System.Collections.Generic;
using System.Reactive.Disposables;

namespace System.Reactive.Data {
    /// <summary>
    /// Lightweight property broker.
    /// </summary>
    public class ReadOnlyReactiveProperty<T> : IReadOnlyReactiveProperty<T>, IDisposable, IObservable<T>, IObserverLinkedList<T>, IObserver<T>
    {
        internal static IEqualityComparer<T> defaultEqualityComparer = EqualityComparer<T>.Default;

        readonly bool distinctUntilChanged = true;
        bool canPublishValueOnSubscribe = false;
        bool isDisposed = false;
        bool isSourceCompleted = false;

        T latestValue = default(T);
        Exception lastException = null;
        IDisposable sourceConnection = null;

        ObserverNode<T> root;
        ObserverNode<T> last;

        public T Value
        {
            get
            {
                return latestValue;
            }
        }

        public bool HasValue
        {
            get
            {
                return canPublishValueOnSubscribe;
            }
        }

        protected virtual IEqualityComparer<T> EqualityComparer
        {
            get
            {
                return defaultEqualityComparer;
            }
        }

        public ReadOnlyReactiveProperty(IObservable<T> source)
        {
            this.sourceConnection = source.Subscribe(this);
        }

        public ReadOnlyReactiveProperty(IObservable<T> source, bool distinctUntilChanged)
        {
            this.distinctUntilChanged = distinctUntilChanged;
            this.sourceConnection = source.Subscribe(this);
        }

        public ReadOnlyReactiveProperty(IObservable<T> source, T initialValue)
        {
            this.latestValue = initialValue;
            this.canPublishValueOnSubscribe = true;
            this.sourceConnection = source.Subscribe(this);
        }

        public ReadOnlyReactiveProperty(IObservable<T> source, T initialValue, bool distinctUntilChanged)
        {
            this.distinctUntilChanged = distinctUntilChanged;
            this.latestValue = initialValue;
            this.canPublishValueOnSubscribe = true;
            this.sourceConnection = source.Subscribe(this);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (lastException != null)
            {
                observer.OnError(lastException);
                return Disposable.Empty;
            }

            if (isSourceCompleted)
            {
                if (canPublishValueOnSubscribe)
                {
                    observer.OnNext(latestValue);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
                else
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
            }

            if (isDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            if (canPublishValueOnSubscribe)
            {
                observer.OnNext(latestValue);
            }

            // subscribe node, node as subscription.
            var next = new ObserverNode<T>(this, observer);
            if (root == null)
            {
                root = last = next;
            }
            else
            {
                last.Next = next;
                next.Previous = last;
                last = next;
            }

            return next;
        }

        public void Dispose() {
            if (isDisposed) return;
            sourceConnection.Dispose();

            var node = root;
            root = last = null;
            isDisposed = true;

            while (node != null) {
                node.OnCompleted();
                node = node.Next;
            }
            GC.SuppressFinalize(this);
        }

        void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
        {
            if (node == root)
            {
                root = node.Next;
            }
            if (node == last)
            {
                last = node.Previous;
            }

            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }
            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }
        }

        void IObserver<T>.OnNext(T value)
        {
            if (isDisposed) return;

            if (canPublishValueOnSubscribe)
            {
                if (distinctUntilChanged && EqualityComparer.Equals(this.latestValue, value))
                {
                    return;
                }
            }

            canPublishValueOnSubscribe = true;

            // SetValue
            this.latestValue = value;

            // call source.OnNext
            var node = root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }
        }

        void IObserver<T>.OnError(Exception error)
        {
            lastException = error;

            // call source.OnError
            var node = root;
            while (node != null)
            {
                node.OnError(error);
                node = node.Next;
            }

            root = last = null;
        }

        void IObserver<T>.OnCompleted()
        {
            isSourceCompleted = true;
            root = last = null;
        }

        public override string ToString()
        {
            return (latestValue == null) ? "(null)" : latestValue.ToString();
        }
    }
}
