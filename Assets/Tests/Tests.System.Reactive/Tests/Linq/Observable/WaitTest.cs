// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Rx.Unity.Tests.Helper;

namespace ReactiveTests.Tests
{
    public partial class WaitTest : ReactiveTest
    {

        [Test]
        public void Wait_ArgumentChecking()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.Wait(default(IObservable<int>)));
        }

#if !NO_THREAD
        [Test]
        public void Wait_Return()
        {
            var x = 42;
            var xs = Observable.Return(x, Rx.Unity.Concurrency.ThreadPoolOnlyScheduler.Instance);
            var res = xs.Wait();
            XunitAssert.Equal(x, res);
        }
#endif

        [Test]
        public void Wait_Empty()
        {
            ReactiveAssert.Throws<InvalidOperationException>(() => Observable.Empty<int>().Wait());
        }

        [Test]
        public void Wait_Throw()
        {
            var ex = new Exception();

            var xs = Observable.Throw<int>(ex);

            ReactiveAssert.Throws(ex, () => xs.Wait());
        }

#if !NO_THREAD
        [Test]
        public void Wait_Range()
        {
            var n = 42;
            var xs = Observable.Range(1, n, Rx.Unity.Concurrency.ThreadPoolOnlyScheduler.Instance);
            var res = xs.Wait();
            XunitAssert.Equal(n, res);
        }
#endif

    }
}
