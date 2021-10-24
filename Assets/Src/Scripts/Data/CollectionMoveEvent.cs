using System.Collections.Generic;

namespace System.Reactive.Data {
    public struct CollectionMoveEvent<T> : IEquatable<CollectionMoveEvent<T>>
    {
        public int OldIndex { get; private set; }
        public int NewIndex { get; private set; }
        public T Value { get; private set; }

        public CollectionMoveEvent(int oldIndex, int newIndex, T value)
            : this()
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("OldIndex:{0} NewIndex:{1} Value:{2}", OldIndex, NewIndex, Value);
        }

        public override int GetHashCode()
        {
            return OldIndex.GetHashCode() ^ NewIndex.GetHashCode() << 2 ^ EqualityComparer<T>.Default.GetHashCode(Value) >> 2;
        }

        public bool Equals(CollectionMoveEvent<T> other)
        {
            return OldIndex.Equals(other.OldIndex) && NewIndex.Equals(other.NewIndex) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }
    }
}
