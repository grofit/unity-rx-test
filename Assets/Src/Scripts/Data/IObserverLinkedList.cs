namespace System.Reactive.Data {
    internal interface IObserverLinkedList<T>
    {
        void UnsubscribeNode(ObserverNode<T> node);
    }
}
