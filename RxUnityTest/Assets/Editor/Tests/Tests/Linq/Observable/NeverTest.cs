﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information. 

using System;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace ReactiveTests.Tests
{
    public class NeverTest : ReactiveTest
    {

        [Test]
        public void Never_ArgumentChecking()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.Never<int>().Subscribe(null));
            ReactiveAssert.Throws<ArgumentNullException>(() => Observable.Never(42).Subscribe(null));
        }

        [Test]
        public void Never_Basic()
        {
            var scheduler = new TestScheduler();

            var xs = Observable.Never<int>();

            var res = scheduler.CreateObserver<int>();

            xs.Subscribe(res);

            scheduler.Start();

            res.Messages.AssertEqual(
            );
        }

        [Test]
        public void Never_Basic_Witness()
        {
            var scheduler = new TestScheduler();

            var xs = Observable.Never(42);

            var res = scheduler.CreateObserver<int>();

            xs.Subscribe(res);

            scheduler.Start();

            res.Messages.AssertEqual(
            );
        }

    }
}
