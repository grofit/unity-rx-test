using System;
using NUnit.Framework;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Unity;
using System.Collections.Generic;

namespace UniRx.Tests {
    public class __UnityReactiveTest {
        [OneTimeSetUp]
        public void Init() {
            ReactiveUnity.SetupPatches();
        }

        [Test]
        public void NormalUsageScenarioTest() {
            Observable.Range(0, 10)
                .Select(x => (x, $"HELLO NR {x}!", new List<int> { x }))
                .CombineLatest(Observable.Range(0, 10).Select(x => x / 2).DistinctUntilChanged(), (first, second) => (first.x, first.Item2, first.Item3, second))
                .Where(x => (x.x % 2 == 0))
                .TakeUntil(Observable.Never<Unit>())
                .Skip(3)
                .Take(1)
                .Subscribe(onNext: test => {
                    Assert.AreEqual(4, test.x);
                    Assert.AreEqual(1, test.second);
                });
        }
    }
}
