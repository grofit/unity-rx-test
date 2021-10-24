using System.Collections.Generic;

namespace System.Reactive.Data {
    public struct CollectionRemoveEvent<T> : IEquatable<CollectionRemoveEvent<T>>
    {
        public int Index { get; private set; }
        public T Value { get; private set; }

        public CollectionRemoveEvent(int index, T value)
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

        public bool Equals(CollectionRemoveEvent<T> other)
        {
            return Index.Equals(other.Index) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }
}
