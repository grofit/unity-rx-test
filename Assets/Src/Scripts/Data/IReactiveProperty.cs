namespace System.Reactive.Data {
    public interface IReactiveProperty<T> : IReadOnlyReactiveProperty<T>
    {
        new T Value { get; set; }
    }
}
