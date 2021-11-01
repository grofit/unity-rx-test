// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

#if !NO_THREAD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace ReactiveTests.Tests
{

    public partial class ThreadPoolSchedulerTest
    {
        [Test]
        public void Schedule_ArgumentChecking()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => ThreadPoolScheduler.Instance.Schedule(42, default));
            ReactiveAssert.Throws<ArgumentNullException>(() => ThreadPoolScheduler.Instance.Schedule(42, DateTimeOffset.Now, default));
            ReactiveAssert.Throws<ArgumentNullException>(() => ThreadPoolScheduler.Instance.Schedule(42, TimeSpan.Zero, default));
        }

        [Test]
        public void Get_Now()
        {
            var res = ThreadPoolScheduler.Instance.Now - DateTime.Now;
            Assert.True(res.Seconds < 1);
        }

        [Test]
        public void ScheduleAction()
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            var nt = ThreadPoolScheduler.Instance;
            var evt = new ManualResetEvent(false);
            nt.Schedule(() => { Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId); evt.Set(); });
            evt.WaitOne();
        }

        [Test]
        public void ProperRooting_NoGC_SingleShot()
        {
            var cts = new CancellationTokenSource();

            new Thread(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    Thread.Sleep(50);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }).Start();

            var tp = ThreadPoolScheduler.Instance;
            var N = 100;
            var cd = new CountdownEvent(N);
            for (var i = 0; i < N; i++)
            {
                tp.Schedule(TimeSpan.FromMilliseconds(100 + i), () => { cd.Signal(); });
            }

            Assert.True(cd.Wait(TimeSpan.FromMinutes(1)));
            cts.Cancel();
        }

        [Test]
        public void ProperRooting_NoGC_Periodic()
        {
            var cts = new CancellationTokenSource();

            new Thread(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    Thread.Sleep(50);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }).Start();

            var tp = ThreadPoolScheduler.Instance;
            var N = 5;
            var e = new ManualResetEvent(false);
            var n = 0;
            var d = tp.SchedulePeriodic(TimeSpan.FromMilliseconds(80), () => { if (Interlocked.Increment(ref n) == N) { e.Set(); } });

            Assert.True(e.WaitOne(TimeSpan.FromMinutes(1)));

            d.Dispose();

            cts.Cancel();
        }

        [Test]
        public void ScheduleActionDueRelative()
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            var nt = ThreadPoolScheduler.Instance;
            var evt = new ManualResetEvent(false);
            nt.Schedule(TimeSpan.FromSeconds(0.2), () => { Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId); evt.Set(); });
            evt.WaitOne();
        }

        [Test]
        public void ScheduleActionDue0()
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            var nt = ThreadPoolScheduler.Instance;
            var evt = new ManualResetEvent(false);
            nt.Schedule(TimeSpan.FromTicks(0), () => { Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId); evt.Set(); });
            evt.WaitOne();
        }

        [Test]
        public void ScheduleActionDueAbsolute()
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            var nt = ThreadPoolScheduler.Instance;
            var evt = new ManualResetEvent(false);
            nt.Schedule(DateTimeOffset.UtcNow + TimeSpan.FromSeconds(0.2), () => { Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId); evt.Set(); });
            evt.WaitOne();
        }

        [Test]
        public void ScheduleActionCancel()
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            var nt = ThreadPoolScheduler.Instance;
            var set = false;
            var d = nt.Schedule(TimeSpan.FromSeconds(0.2), () => { Assert.True(false); set = true; });
            d.Dispose();
            Thread.Sleep(400);
            Assert.False(set);
        }

#if !NO_PERF

        [Test]
        public void ScheduleLongRunning_ArgumentChecking()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => ThreadPoolScheduler.Instance.ScheduleLongRunning(42, default));
        }

        [Test]
        public void ScheduleLongRunning()
        {
            var id = Thread.CurrentThread.ManagedThreadId;
            var nt = ThreadPoolScheduler.Instance;
            var evt = new ManualResetEvent(false);
            nt.ScheduleLongRunning(42, (x, cancel) => { Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId); evt.Set(); });
            evt.WaitOne();
        }

        [Test]
        public void ScheduleLongRunningCancel()
        {
            var nt = ThreadPoolScheduler.Instance;

            var started = new ManualResetEvent(false);
            var stopped = new ManualResetEvent(false);

            var n = 0;

            var d = nt.ScheduleLongRunning(42, (x, cancel) =>
            {
                for (n = 0; !cancel.IsDisposed; n++)
                {
                    if (n == 10)
                    {
                        started.Set();
                    }
                }

                stopped.Set();
            });

            started.WaitOne();
            d.Dispose();

            stopped.WaitOne();
            Assert.True(n >= 10);
        }

        [Test]
        public void Stopwatch()
        {
            var nt = ThreadPoolScheduler.Instance;

            var sw = nt.StartStopwatch();

            var s0 = sw.Elapsed.Ticks;
            Thread.Sleep(10);
            var s1 = sw.Elapsed.Ticks;

            Assert.True(s1 > s0);
        }

        [Test]
        public void Periodic_ArgumentChecking()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => ThreadPoolScheduler.Instance.SchedulePeriodic(0, TimeSpan.FromSeconds(1), null));
            ReactiveAssert.Throws<ArgumentOutOfRangeException>(() => ThreadPoolScheduler.Instance.SchedulePeriodic(0, TimeSpan.FromSeconds(-1), _ => _));
        }

        [Test]
        public void Periodic_Regular()
        {
            Periodic_Impl(TimeSpan.FromMilliseconds(25));
        }

        [Test]
        public void Periodic_Zero()
        {
            Periodic_Impl(TimeSpan.Zero);
        }

        private void Periodic_Impl(TimeSpan period)
        {
            var gate = new object();
            var n = 0;
            var e = new ManualResetEvent(false);

            var lst = new List<int>();

            var d = ThreadPoolScheduler.Instance.SchedulePeriodic(0, period, x =>
            {
                lock (gate)
                {
                    if (n++ == 10)
                    {
                        e.Set();
                    }
                }

                lst.Add(x);
                return x + 1;
            });

            e.WaitOne();
            d.Dispose();

            var m = default(int);
            var k = default(int);

            var i = 0;
            do
            {
                lock (gate)
                {
                    m = n;
                }

                Thread.Sleep(50);

                lock (gate)
                {
                    k = n;
                }
            } while (m != k && i++ < 10); // Wait for Dispose to reach the timer; should be almost instantaneous due to nop'ing out of the action.

            Assert.AreNotEqual(10, i);

            var res = lst.ToArray();
            Assert.True(res.Length >= 10);
            Assert.True(res.Take(10).SequenceEqual(Enumerable.Range(0, 10)));
        }

        [Test]
        public void Periodic_NonReentrant()
        {
            var n = 0;
            var fail = false;

            var d = ThreadPoolScheduler.Instance.SchedulePeriodic(0, TimeSpan.FromMilliseconds(50), x =>
            {
                try
                {
                    if (Interlocked.Increment(ref n) > 1) // Without an AsyncLock this would fail.
                    {
                        fail = true;
                    }

                    Thread.Sleep(100);

                    return x + 1;
                }
                finally
                {
                    Interlocked.Decrement(ref n);
                }
            });

            Thread.Sleep(500);
            d.Dispose();

            Assert.False(fail);
        }

#endif

#if DESKTOPCLR
        // [Trait("SkipCI", "true")]
        [Test]
        public void No_ThreadPool_Starvation_Dispose()
        {
            ThreadPool.GetAvailableThreads(out var bwt, out var bio);

            var N = Environment.ProcessorCount * 2;

            for (var i = 0; i < N; i++)
            {
                var e = new ManualResetEvent(false);
                var f = new ManualResetEvent(false);
                var d = ThreadPoolScheduler.Instance.Schedule(TimeSpan.FromMilliseconds(1), () => { e.Set(); f.WaitOne(); });
                e.WaitOne();
                d.Dispose();
                f.Set();
            }
            ThreadPool.GetAvailableThreads(out var ewt, out var eio);

            Assert.False(bwt - ewt >= N);
        }
#endif
    }
}
#endif
