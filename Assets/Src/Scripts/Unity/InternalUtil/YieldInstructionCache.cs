using UnityEngine;

namespace System.Reactive.Unity.InternalUtil {
    internal static class YieldInstructionCache {
        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
    }
}
