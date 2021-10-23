using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Unity.Data;
using System.Reactive.Unity.InternalUtil;
using UnityEngine;
using System.Reactive.Concurrency;

namespace System.Reactive.Unity {
    public static class ReactiveUnity {
        public static void SetupPatches() {
            
        }

        private static void InitSchedulerDefaults() {
#if WEB_GL
                    // WebGL does not support threadpool
                    return asyncConversions ?? (asyncConversions = Scheduler.MainThread);
#else
            SchedulerDefaults.AsyncConversions = ThreadPoolScheduler.Instance;
#endif
        }

        private static void AddUnityEqualityComparer<T>() {
            var comparer = UnityEqualityComparer.GetDefault<T>();
            ReactiveProperty<T>.defaultEqualityComparer = comparer;
            ReadOnlyReactiveProperty<T>.defaultEqualityComparer = comparer;

        }
    }
}
