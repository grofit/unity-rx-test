using System.Collections.Generic;

namespace System.Reactive.Data {
    // IReadOnlyList<out T> is from .NET 4.5
    public interface IReadOnlyReactiveCollection<T> : IEnumerable<T>
    {
        int Count { get; }
        T this[int index] { get; }
        IObservable<CollectionAddEvent<T>> ObserveAdd();
        IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false);
        IObservable<CollectionMoveEvent<T>> ObserveMove();
        IObservable<CollectionRemoveEvent<T>> ObserveRemove();
        IObservable<CollectionReplaceEvent<T>> ObserveReplace();
        IObservable<Unit> ObserveReset();
    }
}
