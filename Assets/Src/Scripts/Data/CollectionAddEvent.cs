using System.Collections.Generic;

namespace System.Reactive.Data {
    public struct CollectionAddEvent<T> : IEquatable<CollectionAddEvent<T>>
    {
        public int Index { get; }
        public T Value { get; }

        public CollectionAddEvent(int index, T value)
            : this()
        {
            Index = index;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("Index:{0} Value:{1}", Index, Value);
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(Value) << 2;
        }

        public bool Equals(CollectionAddEvent<T> other)
        {
            return Index.Equals(other.Index) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }
}
