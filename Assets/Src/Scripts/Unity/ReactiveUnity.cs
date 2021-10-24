using System.Reactive.Data;
using System.Reactive.Unity.InternalUtil;
using UnityEngine;
using System.Reactive.Concurrency;

namespace System.Reactive.Unity {
    public static class ReactiveUnity {
        public static void SetupPatches() {
            InitSchedulerDefaults();
            AddUnityEqualityComparer<Vector2>();
            AddUnityEqualityComparer<Vector3>();
            AddUnityEqualityComparer<Vector4>();
            AddUnityEqualityComparer<Color>();
            AddUnityEqualityComparer<Color32>();
            AddUnityEqualityComparer<Rect>();
            AddUnityEqualityComparer<Bounds>();
            AddUnityEqualityComparer<Quaternion>();
            AddUnityEqualityComparer<Vector2Int>();
            AddUnityEqualityComparer<Vector3Int>();
            AddUnityEqualityComparer<RangeInt>();
            AddUnityEqualityComparer<RectInt>();
            AddUnityEqualityComparer<BoundsInt>();
        }

        private static void InitSchedulerDefaults() {
            SchedulerDefaults.TimeBasedOperations = UnityMainThreadScheduler.Instance;
#if WEB_GL
            SchedulerDefaults.AsyncConversions = UnityMainThreadScheduler.Instance;
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
