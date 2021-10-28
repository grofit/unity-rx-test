using System.Reactive.Concurrency;
using System.Reactive.PlatformServices;
using System.Reactive.Unity.Concurrency;

namespace System.Reactive.Unity.InternalUtil {
    sealed class UnityEnlightenmentProvider : CurrentPlatformEnlightenmentProvider {
        public override T GetService<T>(object[] args) {
            if (typeof(T) == typeof(IScheduler) && args != null && ((string)args[0]) == "ThreadPool")
                return (T)(object)ThreadPoolOnlyScheduler.Instance;
            return base.GetService<T>(args);
        }
    }
}
