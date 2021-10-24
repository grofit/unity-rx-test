using System;
using System.Reactive;

namespace System.Reactive.Data.Operators {
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
}
