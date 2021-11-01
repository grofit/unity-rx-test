// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using ReactiveTests.Dummies;
using NUnit.Framework;

namespace ReactiveTests.Tests
{
    public class FirstTest : ReactiveTest
    {

        [Test]
        public void First_ArgumentChecking()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.First(default(IObservable<int>)));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.First(default(IObservable<int>), _ => true));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.First(DummyObservable<int>.Instance, default));
        }

        [Test]
        public void First_Empty()
        {
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Empty<int>().First());
        }

        [Test]
        public void FirstPredicate_Empty()
        {
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Empty<int>().First(_ => true));
        }

        [Test]
        public void First_Return()
        {
            var value = 42;
            Assert.AreEqual(value, Observable.Return(value).First());
        }

        [Test]
        public void FirstPredicate_Return()
        {
            var value = 42;
            Assert.AreEqual(value, Observable.Return(value).First(i => i % 2 == 0));
        }

        [Test]
        public void FirstPredicate_Return_NoMatch()
        {
            var value = 42;
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Return(value).First(i => i % 2 != 0));
        }

        [Test]
        public void First_Throw()
        {
            var ex = new Exception();

            var xs = Observable.Throw<int>(ex);

            ReactiveAssert.Throws(ex, () => xs.First());
        }

        [Test]
        public void FirstPredicate_Throw()
        {
            var ex = new Exception();

            var xs = Observable.Throw<int>(ex);

            ReactiveAssert.Throws(ex, () => xs.First(_ => true));
        }

        [Test]
        public void First_Range()
        {
            var value = 42;
            Assert.AreEqual(value, Observable.Range(value, 10).First());
        }

        [Test]
        public void FirstPredicate_Range()
        {
            var value = 42;
            Assert.AreEqual(46, Observable.Range(value, 10).First(i => i > 45));
        }

    }
}
