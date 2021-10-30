using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Reactive.Extendibility.Disposables {
    internal static partial class Disposable {
        /// <summary>
        /// Gets the value stored in <paramref name="fieldRef" /> or a no-op-Disposable if
        /// <paramref name="fieldRef" /> was already disposed.
        /// </summary>
        [return: NotNullIfNotNull("fieldRef")]
        internal static IDisposable? GetValueOrDefault([NotNullIfNotNull("fieldRef")] /*in*/ ref IDisposable? fieldRef) {
            var current = Volatile.Read(ref fieldRef);

            return current == BooleanDisposable.True
                ? System.Reactive.Disposables.Disposable.Empty
                : current;
        }

        /// <summary>
        /// Tries to assign <paramref name="value" /> to <paramref name="fieldRef" />.
        /// </summary>
        /// <returns>A <see cref="TrySetSingleResult"/> value indicating the outcome of the operation.</returns>
        internal static TrySetSingleResult TrySetSingle([NotNullIfNotNull("value")] ref IDisposable? fieldRef, IDisposable? value) {
            var old = Interlocked.CompareExchange(ref fieldRef, value, null);
            if (old == null) {
                return TrySetSingleResult.Success;
            }

            if (old != BooleanDisposable.True) {
                return TrySetSingleResult.AlreadyAssigned;
            }

            value?.Dispose();
            return TrySetSingleResult.Disposed;
        }

        /// <summary>
        /// Disposes <paramref name="fieldRef" />. 
        /// </summary>
        internal static void Dispose([NotNullIfNotNull("fieldRef")] ref IDisposable? fieldRef) {
            var old = Interlocked.Exchange(ref fieldRef, BooleanDisposable.True);

            if (old != BooleanDisposable.True) {
                old?.Dispose();
            }
        }
    }
}
