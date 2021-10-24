using System.Collections.Generic;

namespace System.Reactive.Data {
    // IReadOnlyDictionary is from .NET 4.5
    public interface IReadOnlyReactiveDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        int Count { get; }
        TValue this[TKey index] { get; }
        bool ContainsKey(TKey key);
        bool TryGetValue(TKey key, out TValue value);

        IObservable<DictionaryAddEvent<TKey, TValue>> ObserveAdd();
        IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false);
        IObservable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemove();
        IObservable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplace();
        IObservable<Unit> ObserveReset();
    }
}
