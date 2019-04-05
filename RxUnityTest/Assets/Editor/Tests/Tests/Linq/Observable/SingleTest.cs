// Licensed to the .NET Foundation under one or more agreements.
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
    public class SingleTest : ReactiveTest
    {

        [Test]
        public void Single_ArgumentChecking()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.Single(default(IObservable<int>)));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.Single(default(IObservable<int>), _ => true));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.Single(DummyObservable<int>.Instance, default));
        }

        [Test]
        public void Single_Empty()
        {
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Empty<int>().Single());
        }

        [Test]
        public void SinglePredicate_Empty()
        {
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Empty<int>().Single(_ => true));
        }

        [Test]
        public void Single_Return()
        {
            var value = 42;
            Assert.Equals(value, Observable.Return(value).Single());
        }

        [Test]
        public void Single_Throw()
        {
            var ex = new Exception();

            var xs = Observable.Throw<int>(ex);

            ReactiveAssert.Throws(ex, () => xs.Single());
        }

        [Test]
        public void Single_Range()
        {
            var value = 42;
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Range(value, 10).Single());
        }

        [Test]
        public void SinglePredicate_Range()
        {
            var value = 42;
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Range(value, 10).Single(i => i % 2 == 0));
        }

        [Test]
        public void SinglePredicate_Range_ReducesToSingle()
        {
            var value = 42;
            Assert.Equals(45, Observable.Range(value, 10).Single(i => i == 45));
        }


    }
}
