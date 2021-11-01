// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using ReactiveTests.Dummies;
using NUnit.Framework;
using Rx.Unity.Tests.Helper;

namespace ReactiveTests.Tests
{
    public partial class SingleOrDefaultTest : ReactiveTest
    {

        [Test]
        public void SingleOrDefault_ArgumentChecking()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.SingleOrDefault(default(IObservable<int>)));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.SingleOrDefault(default(IObservable<int>), _ => true));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.SingleOrDefault(DummyObservable<int>.Instance, default));
        }

        [Test]
        public void SingleOrDefault_Empty()
        {
            XunitAssert.Equal(default, Observable.Empty<int>().SingleOrDefault());
        }

        [Test]
        public void SingleOrDefaultPredicate_Empty()
        {
            XunitAssert.Equal(default, Observable.Empty<int>().SingleOrDefault(_ => true));
        }

        [Test]
        public void SingleOrDefault_Return()
        {
            var value = 42;
            XunitAssert.Equal(value, Observable.Return(value).SingleOrDefault());
        }

        [Test]
        public void SingleOrDefault_Throw()
        {
            var ex = new Exception();

            var xs = Observable.Throw<int>(ex);

            ReactiveAssert.Throws(ex, () => xs.SingleOrDefault());
        }

        [Test]
        public void SingleOrDefault_Range()
        {
            var value = 42;
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Range(value, 10).SingleOrDefault());
        }

        [Test]
        public void SingleOrDefaultPredicate_Range()
        {
            var value = 42;
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Range(value, 10).SingleOrDefault(i => i % 2 == 0));
        }

        [Test]
        public void SingleOrDefault_Range_ReducesToSingle()
        {
            var value = 42;
            XunitAssert.Equal(45, Observable.Range(value, 10).SingleOrDefault(i => i == 45));
        }

        [Test]
        public void SingleOrDefault_Range_ReducesToNone()
        {
            var value = 42;
            XunitAssert.Equal(0, Observable.Range(value, 10).SingleOrDefault(i => i > 100));
        }

    }
}
