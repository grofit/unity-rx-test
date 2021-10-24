﻿using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Unity.Concurrency;
using System.Reactive.Unity.Linq.Observables;
using System.Reactive.Unity.Triggers;
using System.Threading;
using UnityEngine;

namespace System.Reactive.Unity.Linq {
    public static class UnityObservable {
        public static IObservable<long> EveryUpdate() => new UnityInterval(FrameCountType.Update);

        public static IObservable<long> EveryFixedUpdate() => new UnityInterval(FrameCountType.FixedUpdate);

        public static IObservable<long> EveryEndOfFrame() => new UnityInterval(FrameCountType.EndOfFrame);

        internal static IEnumerator EveryCycleCore(IObserver<long> observer, CancellationToken cancellationToken) {
            if (cancellationToken.IsCancellationRequested) yield break;
            var count = 0L;
            while (true) {
                yield return null;
                if (cancellationToken.IsCancellationRequested) yield break;

                observer.OnNext(count++);
            }
        }

        public static IObservable<T> ObserveOnMainThread<T>(this IObservable<T> source) {
            return source.ObserveOn(UnityMainThreadScheduler.Instance);
        }

        public static IObservable<T> SubscribeOnMainThread<T>(this IObservable<T> source) {
            return source.SubscribeOn(UnityMainThreadScheduler.Instance);
        }

        /// <summary>
        /// Buffer elements in during target frame counts. Default raise same frame of end(frameCount = 0, frameCountType = EndOfFrame).
        /// </summary>
        public static IObservable<IList<T>> BatchFrame<T>(this IObservable<T> source) {
            if (source is null) throw new ArgumentNullException(nameof(source));
            // if use default argument, comiler errors ambiguous(Unity's limitation)
            return new BatchFrame<T>(source, 0, FrameCountType.EndOfFrame);
        }

        /// <summary>
        /// Buffer elements in during target frame counts.
        /// </summary>
        public static IObservable<IList<T>> BatchFrame<T>(this IObservable<T> source, int frameCount, FrameCountType frameCountType) {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (frameCount < 0) throw new ArgumentException("frameCount must be >= 0, frameCount:" + frameCount, nameof(frameCount));
            return new BatchFrame<T>(source, frameCount, frameCountType);
        }

        public static IObservable<T> DelayFrame<T>(this IObservable<T> source, int frameCount, FrameCountType frameCountType = FrameCountType.Update) {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (frameCount < 0) throw new ArgumentException("frameCount must be >= 0, frameCount:" + frameCount, nameof(frameCount));
            return new DelayFrame<T>(source, frameCount, frameCountType);
        }

        public static IObservable<T> TakeUntilDestroy<T>(this IObservable<T> source, Component target) {
            return source.TakeUntil(target.OnDestroyAsObservable());
        }

        public static IObservable<T> TakeUntilDestroy<T>(this IObservable<T> source, GameObject target) {
            return source.TakeUntil(target.OnDestroyAsObservable());
        }
    }
}
