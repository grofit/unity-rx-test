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
    public partial class LastOrDefaultTest : ReactiveTest
    {

        [Test]
        public void LastOrDefault_ArgumentChecking()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.LastOrDefault(default(IObservable<int>)));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.LastOrDefault(default(IObservable<int>), _ => true));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.LastOrDefault(DummyObservable<int>.Instance, default));
        }

        [Test]
        public void LastOrDefault_Empty()
        {
            XunitAssert.Equal(default, Observable.Empty<int>().LastOrDefault());
        }

        [Test]
        public void LastOrDefaultPredicate_Empty()
        {
            XunitAssert.Equal(default, Observable.Empty<int>().LastOrDefault(_ => true));
        }

        [Test]
        public void LastOrDefault_Return()
        {
            var value = 42;
            XunitAssert.Equal(value, Observable.Return(value).LastOrDefault());
        }

        [Test]
        public void LastOrDefault_Throw()
        {
            var ex = new Exception();

            var xs = Observable.Throw<int>(ex);

            ReactiveAssert.Throws(ex, () => xs.LastOrDefault());
        }

        [Test]
        public void LastOrDefault_Range()
        {
            var value = 42;
            XunitAssert.Equal(value, Observable.Range(value - 9, 10).LastOrDefault());
        }

        [Test]
        public void LastOrDefaultPredicate_Range()
        {
            var value = 42;
            XunitAssert.Equal(50, Observable.Range(value, 10).LastOrDefault(i => i % 2 == 0));
        }

    }
}
