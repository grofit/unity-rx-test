using System.Collections.Generic;

namespace System.Reactive.Data {
    public struct CollectionReplaceEvent<T> : IEquatable<CollectionReplaceEvent<T>>
    {
        public int Index { get; private set; }
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }

        public CollectionReplaceEvent(int index, T oldValue, T newValue)
            : this()
        {
            Index = index;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public override string ToString()
        {
            return string.Format("Index:{0} OldValue:{1} NewValue:{2}", Index, OldValue, NewValue);
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(OldValue) << 2 ^ EqualityComparer<T>.Default.GetHashCode(NewValue) >> 2;
        }

        public bool Equals(CollectionReplaceEvent<T> other)
        {
            return Index.Equals(other.Index)
                && EqualityComparer<T>.Default.Equals(OldValue, other.OldValue)
                && EqualityComparer<T>.Default.Equals(NewValue, other.NewValue);
        }
    }
}
