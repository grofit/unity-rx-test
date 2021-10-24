using System.Reactive.Data.Operators;

namespace System.Reactive.Data {
    public static class ObservableExtensions {
        /// <summary>
        /// Converting .Select(_ => Unit.Default) sequence.
        /// </summary>
        public static IObservable<Unit> AsUnitObservable<T>(this IObservable<T> source) {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new AsUnit<T>(source);
        }

        /// <summary>
        /// Same as LastOrDefault().AsUnitObservable().
        /// </summary>
        public static IObservable<Unit> AsSingleUnitObservable<T>(this IObservable<T> source) {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new AsSingleUnit<T>(source);
        }

        /// <summary>Projects old and new element of a sequence into a new form.</summary>
        public static IObservable<Pair<T>> Pairwise<T>(this IObservable<T> source) {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new Pairwise<T>(source);
        }
    }
}
