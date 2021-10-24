using System;
using System.Collections.Generic;
using System.Reactive;

namespace UniRx.Tests {
    public class RecordObserver<T> : IObserver<T>
    {
        readonly object gate = new object();
        readonly IDisposable subscription;

        public List<T> Values { get; set; }
        public List<Notification<T>> Notifications { get; set; }

        public RecordObserver(IDisposable subscription)
        {
            this.subscription = subscription;
            this.Values = new List<T>();
            this.Notifications = new List<Notification<T>>();
        }

        public void DisposeSubscription()
        {
            subscription.Dispose();
        }

        void IObserver<T>.OnNext(T value)
        {
            lock (gate)
            {
                Values.Add(value);
                Notifications.Add(Notification.CreateOnNext<T>(value));
            }
        }

        void IObserver<T>.OnError(Exception error)
        {
            lock (gate)
            {
                Notifications.Add(Notification.CreateOnError<T>(error));
            }
        }
        void IObserver<T>.OnCompleted()
        {
            lock (gate)
            {
                Notifications.Add(Notification.CreateOnCompleted<T>());
            }
        }
    }
}
