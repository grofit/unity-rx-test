using System.Reactive.Data.Linq.Observables;
using System.Reactive.Linq;
using System.Linq;

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

        /// <summary>
        /// same as Publish().RefCount()
        /// </summary>
        public static IObservable<T> Share<T>(this IObservable<T> source) {
            return source.Publish().RefCount();
        }

        public static IObservable<TSource> Concat<TSource>(this IObservable<TSource> first, params IObservable<TSource>[] seconds) {
            if (first is null)
                throw new ArgumentNullException(nameof(first));
            if (seconds is null)
                throw new ArgumentNullException(nameof(seconds));
            return Observable.Concat(new[] { first }.Concat(seconds));
        }

        public static IObservable<T> Merge<T>(this IObservable<T> first, params IObservable<T>[] seconds) {
            if (first is null)
                throw new ArgumentNullException(nameof(first));
            if (seconds is null)
                throw new ArgumentNullException(nameof(seconds));
            return Observable.Merge(new[] { first }.Concat(seconds));
        }
    }
}
