using System.Reactive.PlatformServices;

namespace System.Reactive.Unity.InternalUtil {
    internal class UnityPlatformEnlightenmentProvider : IPlatformEnlightenmentProvider {
        public T GetService<T>(params object[] args) where T : class {
            throw new NotImplementedException();
        }
    }
}
