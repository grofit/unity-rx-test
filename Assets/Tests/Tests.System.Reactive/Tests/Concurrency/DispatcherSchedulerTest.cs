// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

#if HAS_WPF
using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace ReactiveTests.Tests
{
    
    public class DispatcherSchedulerTest : TestBase
    {
        [Test]
        public void Ctor_ArgumentChecking()
        {
            ReactiveAssert.Throws<ArgumentNullException>(() => new DispatcherScheduler(null));
        }

        [Test]
        public void Current()
        {
            using (DispatcherHelpers.RunTest(out var d))
            {
                var e = new ManualResetEvent(false);

                d.BeginInvoke(() =>
                {
                    var c = DispatcherScheduler.Current;
                    c.Schedule(() => { e.Set(); });
                });

                e.WaitOne();
            }
        }

        [Test]
        public void Current_None()
        {
            var e = default(Exception);

            var t = new Thread(() =>
            {
                try
                {
                    var ignored = DispatcherScheduler.Current;
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            });

            t.Start();
            t.Join();

            Assert.True(e != null && e is InvalidOperationException);
        }

        [Test]
        public void Dispatcher()
        {
            using (DispatcherHelpers.RunTest(out var disp))
            {
                Assert.AreSame(disp.Dispatcher, new DispatcherScheduler(disp).Dispatcher);
            }
        }

        [Test]
        public void Now()
        {
            using (DispatcherHelpers.RunTest(out var disp))
            {
                var res = new DispatcherScheduler(disp).Now - DateTime.Now;
                Assert.True(res.Seconds < 1);
            }
        }

        [Test]
        public void Schedule_ArgumentChecking()
        {
            using (DispatcherHelpers.RunTest(out var disp))
            {
                var s = new DispatcherScheduler(disp);
                ReactiveAssert.Throws<ArgumentNullException>(() => s.Schedule(42, default(Func<IScheduler, int, IDisposable>)));
                ReactiveAssert.Throws<ArgumentNullException>(() => s.Schedule(42, TimeSpan.FromSeconds(1), default(Func<IScheduler, int, IDisposable>)));
                ReactiveAssert.Throws<ArgumentNullException>(() => s.Schedule(42, DateTimeOffset.Now, default(Func<IScheduler, int, IDisposable>)));
            }
        }

        [Test]
        [Asynchronous]
        public void Schedule()
        {
            using (DispatcherHelpers.RunTest(out var disp))
            {
                RunAsync(evt =>
                {
                    var id = Thread.CurrentThread.ManagedThreadId;
                    var sch = new DispatcherScheduler(disp);
                    sch.Schedule(() =>
                    {
                        Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId);
                        evt.Set();
                    });
                });
            }
        }

        [Test]
        public void ScheduleError()
        {
            using (DispatcherHelpers.RunTest(out var disp))
            {
                var ex = new Exception();

                var id = Thread.CurrentThread.ManagedThreadId;
                var evt = new ManualResetEvent(false);

                Exception thrownEx = null;
                disp.UnhandledException += (o, e) =>
                {
                    thrownEx = e.Exception;
                    evt.Set();
                    e.Handled = true;
                };
                var sch = new DispatcherScheduler(disp);
                sch.Schedule(() => { throw ex; });
                evt.WaitOne();

                Assert.AreSame(ex, thrownEx);
            }
        }

        [Test]
        public void ScheduleRelative()
        {
            ScheduleRelative_(TimeSpan.FromSeconds(0.2));
        }

        [Test]
        public void ScheduleRelative_Zero()
        {
            ScheduleRelative_(TimeSpan.Zero);
        }

        private void ScheduleRelative_(TimeSpan delay)
        {
            using (DispatcherHelpers.RunTest(out var disp))
            {
                var evt = new ManualResetEvent(false);

                var id = Thread.CurrentThread.ManagedThreadId;

                var sch = new DispatcherScheduler(disp);

                sch.Schedule(delay, () =>
                {
                    Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId);

                    sch.Schedule(delay, () =>
                    {
                        Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId);
                        evt.Set();
                    });
                });

                evt.WaitOne();
            }
        }

        [Test]
        public void ScheduleRelative_Cancel()
        {
            using (DispatcherHelpers.RunTest(out var disp))
            {
                var evt = new ManualResetEvent(false);
                
                var id = Thread.CurrentThread.ManagedThreadId;

                var sch = new DispatcherScheduler(disp);
                
                sch.Schedule(TimeSpan.FromSeconds(0.1), () =>
                {
                    Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId);

                    var d = sch.Schedule(TimeSpan.FromSeconds(0.1), () =>
                    {
                        Assert.True(false);
                        evt.Set();
                    });

                    sch.Schedule(() =>
                    {
                        d.Dispose();
                    });

                    sch.Schedule(TimeSpan.FromSeconds(0.2), () =>
                    {
                        Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId);
                        evt.Set();
                    });
                });

                evt.WaitOne();
            }
        }

        [Test]
        public void SchedulePeriodic_ArgumentChecking()
        {
            using (DispatcherHelpers.RunTest(out var disp))
            {
                var s = new DispatcherScheduler(disp);

                ReactiveAssert.Throws<ArgumentNullException>(() => s.SchedulePeriodic(42, TimeSpan.FromSeconds(1), default(Func<int, int>)));
                ReactiveAssert.Throws<ArgumentOutOfRangeException>(() => s.SchedulePeriodic(42, TimeSpan.FromSeconds(-1), x => x));
            }
        }

        [Test]
        public void SchedulePeriodic()
        {
            using (DispatcherHelpers.RunTest(out var disp))
            {
                var evt = new ManualResetEvent(false);

                var id = Thread.CurrentThread.ManagedThreadId;

                var sch = new DispatcherScheduler(disp);

                var d = new SingleAssignmentDisposable();

                d.Disposable = sch.SchedulePeriodic(1, TimeSpan.FromSeconds(0.1), n =>
                {
                    Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId);

                    if (n == 3)
                    {
                        d.Dispose();

                        sch.Schedule(TimeSpan.FromSeconds(0.2), () =>
                        {
                            Assert.AreNotEqual(id, Thread.CurrentThread.ManagedThreadId);
                            evt.Set();
                        });
                    }

                    if (n > 3)
                    {
                        Assert.True(false);
                    }

                    return n + 1;
                });

                evt.WaitOne();
            }
        }
    }
}
#endif
