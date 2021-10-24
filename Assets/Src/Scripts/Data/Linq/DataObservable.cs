using System.Reactive.Data.Linq.Observables;
using System.Reactive.Linq;

namespace System.Reactive.Data.Linq {
    public static class DataObservable {
        public static IObservable<Unit> ReturnUnit() => Observable.Return(Unit.Default);

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
