using System.Collections.Generic;

namespace System.Reactive.Data {
    public struct DictionaryRemoveEvent<TKey, TValue> : IEquatable<DictionaryRemoveEvent<TKey, TValue>>
    {
        public TKey Key { get; private set; }
        public TValue Value { get; private set; }

        public DictionaryRemoveEvent(TKey key, TValue value)
            : this()
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("Key:{0} Value:{1}", Key, Value);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TKey>.Default.GetHashCode(Key) ^ EqualityComparer<TValue>.Default.GetHashCode(Value) << 2;
        }

        public bool Equals(DictionaryRemoveEvent<TKey, TValue> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }
    }
}
