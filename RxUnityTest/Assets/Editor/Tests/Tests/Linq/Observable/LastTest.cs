﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information. 

using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using ReactiveTests.Dummies;
using NUnit.Framework;

namespace ReactiveTests.Tests
{
    public class LastTest : ReactiveTest
    {

        [Test]
        public void Last_ArgumentChecking()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.Last(default(IObservable<int>)));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.Last(default(IObservable<int>), _ => true));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.Last(DummyObservable<int>.Instance, default));
        }

        [Test]
        public void Last_Empty()
        {
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Empty<int>().Last());
        }

        [Test]
        public void LastPredicate_Empty()
        {
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Empty<int>().Last(_ => true));
        }

        [Test]
        public void Last_Return()
        {
            var value = 42;
            Assert.AreEqual(value, Observable.Return(value).Last());
        }

        [Test]
        public void Last_Throw()
        {
            var ex = new Exception();

            var xs = Observable.Throw<int>(ex);

            ReactiveAssert.Throws(ex, () => xs.Last());
        }

        [Test]
        public void Last_Range()
        {
            var value = 42;
            Assert.AreEqual(value, Observable.Range(value - 9, 10).Last());
        }

        [Test]
        public void LastPredicate_Range()
        {
            var value = 42;
            Assert.AreEqual(50, Observable.Range(value, 10).Last(i => i % 2 == 0));
        }

    }
}
