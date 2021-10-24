using System.Collections.Generic;

namespace System.Reactive.Data {
    // Pair is used for Observable.Pairwise
    [Serializable]
    public struct Pair<T> : IEquatable<Pair<T>>
    {
        public T Previous { get; }
        public T Current { get; }

        public Pair(T previous, T current)
        {
            Previous = previous;
            Current = current;
        }

        public override int GetHashCode()
        {
            var comparer = EqualityComparer<T>.Default;
            var h0 = comparer.GetHashCode(Previous);
            return (h0 << 5) + h0 ^ comparer.GetHashCode(Current);
        }

        public override bool Equals(object obj) {
            return obj is Pair<T> && Equals((Pair<T>)obj);
        }

        public bool Equals(Pair<T> other)
        {
            var comparer = EqualityComparer<T>.Default;

            return comparer.Equals(Previous, other.Previous) &&
                comparer.Equals(Current, other.Current);
        }

        public override string ToString() => $"({Previous}, {Current})";
    }
}
