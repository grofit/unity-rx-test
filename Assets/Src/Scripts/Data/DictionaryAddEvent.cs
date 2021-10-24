using System.Collections.Generic;

namespace System.Reactive.Data {
    public struct DictionaryAddEvent<TKey, TValue> : IEquatable<DictionaryAddEvent<TKey, TValue>>
    {
        public TKey Key { get; private set; }
        public TValue Value { get; private set; }

        public DictionaryAddEvent(TKey key, TValue value)
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

        public bool Equals(DictionaryAddEvent<TKey, TValue> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }
    }
}
