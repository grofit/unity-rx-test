using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace UniRx.Tests {
    public static class TestUtil
    {
        public static T[] ToArrayWait<T>(this IObservable<T> source)
        {
            return source.ToArray().Wait();
        }

        public static RecordObserver<T> Record<T>(this IObservable<T> source)
        {
            var d = new SingleAssignmentDisposable();
            var observer = new RecordObserver<T>(d);
            d.Disposable = source.Subscribe(observer);

            return observer;
        }

        public static void SetSchedulerForImport() {
            SchedulerDefaults.TimeBasedOperations = DefaultScheduler.Instance;
            SchedulerDefaults.AsyncConversions = DefaultScheduler.Instance;
        }
    }
}
