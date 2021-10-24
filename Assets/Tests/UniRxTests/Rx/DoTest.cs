using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NUnit.Framework;

namespace UniRx.Tests.Operators {

    public class DoTest
    {
        class ListObserver : IObserver<int>
        {
            public List<int> list = new List<int>();

            public void OnCompleted()
            {
                list.Add(1000);
            }

            public void OnError(Exception error)
            {
                list.Add(100);
            }

            public void OnNext(int value)
            {
                list.Add(value);
            }
        }

        [Test]
        public void Do()
        {
            var list = new List<int>();
            Observable.Empty<int>().Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).DefaultIfEmpty().Wait();
            list.Is(1000);

            list.Clear();
            Observable.Range(1, 5).Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).Wait();
            list.Is(1, 2, 3, 4, 5, 1000);

            list.Clear();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).Do(x => list.Add(x), ex => list.Add(100), () => list.Add(1000)).Subscribe(_ => { }, _ => { }, () => { });
            list.Is(1, 2, 3, 4, 5, 100);
        }

        [Test]
        public void DoObserver()
        {
            var observer = new ListObserver();
            Observable.Empty<int>().Do(observer).DefaultIfEmpty().Wait();
            observer.list.Is(1000);

            observer = new ListObserver();
            Observable.Range(1, 5).Do(observer).Wait();
            observer.list.Is(1, 2, 3, 4, 5, 1000);

            observer = new ListObserver();
            Observable.Range(1, 5).Concat(Observable.Throw<int>(new Exception())).Do(observer).Subscribe(_ => { }, _ => { }, () => { });
            observer.list.Is(1, 2, 3, 4, 5, 100);
        }
    }
}
