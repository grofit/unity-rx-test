// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System;
using System.Reactive;
using System.Reactive.Linq;
using Rx.Unity;
using System.Threading.Tasks;
using NUnit.Framework;
using UniRx.Tests;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.TestTools;

namespace Tests.System.Reactive.Tests
{
    public class TaskLikeSupportTest {
        [SetUp]
        public void Init() {
            TestUtil.SetSchedulerForImport();
        }

        [TearDown]
        public void Dispose() {
            ReactiveUnity.SetupPatches();
        }

        // Rx.Unity - not supported for unity:
        // async ITaskObservable<int> stuff
        //        [Test]
        //        [Category("Task")]
        //        public void Return() {
        //            Assert.AreEqual(42, ManOrBoy_Return().Wait());
        //        }

        //#pragma warning disable 1998
        //        private async ITaskObservable<int> ManOrBoy_Return()
        //        {
        //            return 42;
        //        }
        //#pragma warning restore 1998

        [Test]
        [Category("async")]
        public void Throw()
        {
            Assert.Throws<DivideByZeroException>(() => ManOrBoy_Throw(42, 0).Wait());
        }

#pragma warning disable 1998
        private async ITaskObservable<int> ManOrBoy_Throw(int n, int d)
        {
            return ManOrBoy_ThrowNonAsync(n, d);
        }
#pragma warning restore 1998

        [Il2CppSetOption(Option.DivideByZeroChecks, true)]
        private static int ManOrBoy_ThrowNonAsync(int n, int d) => n / d;

        // Rx.Unity - not supported for unity:
        // async ITaskObservable<int> stuff
//        [Test]
//        [Category("Task")]
//        public void Basics() {
//            Assert.AreEqual(45, ManOrBoy_Basics().Wait());
//        }

//#pragma warning disable 1998
//        private async ITaskObservable<int> ManOrBoy_Basics()
//        {
//            var res = 0;

//            for (var i = 0; i < 10; i++)
//            {
//                switch (i % 4)
//                {
//                    case 0:
//                        res += await Observable.Return(i);
//                        break;
//                    case 1:
//                        res += await Observable.Return(i).Delay(TimeSpan.FromMilliseconds(50));
//                        break;
//                    case 2:
//                        res += await Task.FromResult(i);
//                        break;
//                    case 3:
//                        res += await Task.Run(() => { Task.Delay(50).Wait(); return i; });
//                        break;
//                }
//            }

//            return res;
//        }
//#pragma warning restore 1998
    }
}
