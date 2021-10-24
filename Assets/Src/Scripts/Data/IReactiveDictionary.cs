using System.Collections.Generic;

namespace System.Reactive.Data {
    public interface IReactiveDictionary<TKey, TValue> : IReadOnlyReactiveDictionary<TKey, TValue>, IDictionary<TKey, TValue>
    {
    }
}
