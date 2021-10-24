using System.Collections.Generic;
using System.Reactive.Disposables;

namespace System.Reactive.Data {

    /// <summary>
    /// Lightweight property broker.
    /// </summary>
    public class ReactiveProperty<T> : IReactiveProperty<T>, IDisposable, IObservable<T>, IObserverLinkedList<T>
    {
        internal static IEqualityComparer<T> defaultEqualityComparer = EqualityComparer<T>.Default;

        T value = default(T);
        ObserverNode<T> root;
        ObserverNode<T> last;
        bool isDisposed = false;

        protected virtual IEqualityComparer<T> EqualityComparer
        {
            get
            {
                return defaultEqualityComparer;
            }
        }

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!EqualityComparer.Equals(this.value, value))
                {
                    SetValue(value);
                    if (isDisposed)
                        return;

                    RaiseOnNext(ref value);
                }
            }
        }

        // always true, allows empty constructor 'can' publish value on subscribe.
        // because sometimes value is deserialized from UnityEngine.
        public bool HasValue
        {
            get
            {
                return true;
            }
        }

        public ReactiveProperty()
            : this(default(T))
        {
        }

        public ReactiveProperty(T initialValue)
        {
            SetValue(initialValue);
        }

        void RaiseOnNext(ref T value)
        {
            var node = root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }
        }

        protected virtual void SetValue(T value)
        {
            this.value = value;
        }

        public void SetValueAndForceNotify(T value)
        {
            SetValue(value);
            if (isDisposed)
                return;

            RaiseOnNext(ref value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (isDisposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // raise latest value on subscribe
            observer.OnNext(value);

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            var node = root;
            root = last = null;
            isDisposed = true;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
        }

        public override string ToString()
        {
            return (value == null) ? "(null)" : value.ToString();
        }
    }
}
