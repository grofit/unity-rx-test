using System.Collections.Generic;
using System.Reactive.Linq;

namespace System.Reactive.Data {
    /// <summary>
    /// Extension methods of ReactiveProperty&lt;T&gt;
    /// </summary>
    public static class ReactivePropertyExtensions
    {
        public static IReadOnlyReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source)
        {
            return new ReadOnlyReactiveProperty<T>(source);
        }

        public static IReadOnlyReactiveProperty<T> ToReactiveProperty<T>(this IObservable<T> source, T initialValue)
        {
            return new ReadOnlyReactiveProperty<T>(source, initialValue);
        }

        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> source)
        {
            return new ReadOnlyReactiveProperty<T>(source);
        }

        /// <summary>
        /// Create ReadOnlyReactiveProperty with distinctUntilChanged: false.
        /// </summary>
        public static ReadOnlyReactiveProperty<T> ToSequentialReadOnlyReactiveProperty<T>(this IObservable<T> source)
        {
            return new ReadOnlyReactiveProperty<T>(source, distinctUntilChanged: false);
        }

        public static ReadOnlyReactiveProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> source, T initialValue)
        {
            return new ReadOnlyReactiveProperty<T>(source, initialValue);
        }

        /// <summary>
        /// Create ReadOnlyReactiveProperty with distinctUntilChanged: false.
        /// </summary>
        public static ReadOnlyReactiveProperty<T> ToSequentialReadOnlyReactiveProperty<T>(this IObservable<T> source, T initialValue)
        {
            return new ReadOnlyReactiveProperty<T>(source, initialValue, distinctUntilChanged: false);
        }

        public static IObservable<T> SkipLatestValueOnSubscribe<T>(this IReadOnlyReactiveProperty<T> source)
        {
            return source.HasValue ? source.Skip(1) : source;
        }

        // for multiple toggle or etc..

        /// <summary>
        /// Lastest values of each sequence are all true.
        /// </summary>
        public static IObservable<bool> CombineLatestValuesAreAllTrue(this IEnumerable<IObservable<bool>> sources)
        {
            return sources.CombineLatest().Select(xs =>
            {
                foreach (var item in xs)
                {
                    if (item == false)
                        return false;
                }
                return true;
            });
        }


        /// <summary>
        /// Lastest values of each sequence are all false.
        /// </summary>
        public static IObservable<bool> CombineLatestValuesAreAllFalse(this IEnumerable<IObservable<bool>> sources)
        {
            return sources.CombineLatest().Select(xs =>
            {
                foreach (var item in xs)
                {
                    if (item == true)
                        return false;
                }
                return true;
            });
        }
    }
}
