using System;
using System.Reactive;

namespace UniRx.Operators {
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

        internal sealed class AsUnit<T> : Producer<Unit, AsUnit<T>._> {
            private readonly IObservable<T> _source;

            public AsUnit(IObservable<T> source) => _source = source;

            protected override _ CreateSink(IObserver<Unit> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<T, Unit> {
                public _(IObserver<Unit> observer) : base(observer) { }

                public override void OnNext(T value) => ForwardOnNext(Unit.Default);
            }
        }

        internal sealed class AsSingleUnit<T> : Producer<Unit, AsSingleUnit<T>._> {
            private readonly IObservable<T> _source;

            public AsSingleUnit(IObservable<T> source) => _source = source;

            protected override _ CreateSink(IObserver<Unit> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<T, Unit> {
                public _(IObserver<Unit> observer) : base(observer) {}

                public override void OnNext(T value) { }

                public override void OnCompleted() {
                    ForwardOnNext(Unit.Default);
                    ForwardOnCompleted();
                }
            }
        }

        internal sealed class Pairwise<T> : Producer<Pair<T>, Pairwise<T>._> {
            private readonly IObservable<T> _source;

            public Pairwise(IObservable<T> source) => _source = source;

            protected override _ CreateSink(IObserver<Pair<T>> observer) => new _(observer);

            protected override void Run(_ sink) => sink.Run(_source);

            internal sealed class _ : Sink<T, Pair<T>> {
                private T _prev;
                private bool _isFirst = true;

                public _(IObserver<Pair<T>> observer) : base(observer) { }

                public override void OnNext(T value) {
                    if (_isFirst) {
                        _isFirst = false;
                        _prev = value;
                        return;
                    }

                    var pair = new Pair<T>(_prev, value);
                    _prev = value;
                    ForwardOnNext(pair);
                }
            }
        }
    }
}
