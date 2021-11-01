// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace ReactiveTests.Tests
{
    public class ToEventTest : ReactiveTest
    {

        [Test]
        public void ToEvent_ArgumentChecks()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.ToEvent(default));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.ToEvent(default(IObservable<int>)));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.ToEvent(default(IObservable<EventPattern<EventArgs>>)));
        }

        [Test]
        public void ToEvent_Unit()
        {
            var src = new Subject<Unit>();
            var evt = src.ToEvent();

            var num = 0;
            var hnd = new Action<Unit>(_ =>
            {
                num++;
            });

            evt.OnNext += hnd;

            Assert.AreEqual(0, num);

            src.OnNext(new Unit());
            Assert.AreEqual(1, num);

            src.OnNext(new Unit());
            Assert.AreEqual(2, num);

            evt.OnNext -= hnd;

            src.OnNext(new Unit());
            Assert.AreEqual(2, num);
        }

        [Test]
        public void ToEvent_NonUnit()
        {
            var src = new Subject<int>();
            var evt = src.ToEvent();

            var lst = new List<int>();
            var hnd = new Action<int>(e =>
            {
                lst.Add(e);
            });

            evt.OnNext += hnd;

            src.OnNext(1);
            src.OnNext(2);

            evt.OnNext -= hnd;

            src.OnNext(3);

            Assert.True(lst.SequenceEqual(new[] { 1, 2 }));
        }

        [Test]
        public void ToEvent_FromEvent()
        {
            var src = new Subject<int>();
            var evt = src.ToEvent();

            var res = Observable.FromEvent<int>(h => evt.OnNext += h, h => evt.OnNext -= h);

            var lst = new List<int>();
            using (res.Subscribe(e => lst.Add(e), () => Assert.True(false)))
            {
                src.OnNext(1);
                src.OnNext(2);
            }

            src.OnNext(3);

            Assert.True(lst.SequenceEqual(new[] { 1, 2 }));
        }

    }
}
