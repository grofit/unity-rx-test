using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace System.Reactive.Data {
    public class ReactiveDictionary<TKey, TValue> : IReactiveDictionary<TKey, TValue>, IDictionary<TKey, TValue>, IEnumerable, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, IDisposable
    {
        private bool _isDisposed = false;
        private readonly Dictionary<TKey, TValue> _inner;

        public ReactiveDictionary()
        {
            _inner = new Dictionary<TKey, TValue>();
        }

        public ReactiveDictionary(IEqualityComparer<TKey> comparer)
        {
            _inner = new Dictionary<TKey, TValue>(comparer);
        }

        public ReactiveDictionary(Dictionary<TKey, TValue> innerDictionary)
        {
            _inner = innerDictionary;
        }

        public TValue this[TKey key]
        {
            get
            {
                return _inner[key];
            }

            set
            {
                TValue oldValue;
                if (TryGetValue(key, out oldValue))
                {
                    _inner[key] = value;
                    if (dictionaryReplace != null) dictionaryReplace.OnNext(new DictionaryReplaceEvent<TKey, TValue>(key, oldValue, value));
                }
                else
                {
                    _inner[key] = value;
                    if (dictionaryAdd != null) dictionaryAdd.OnNext(new DictionaryAddEvent<TKey, TValue>(key, value));
                    if (countChanged != null) countChanged.OnNext(Count);
                }
            }
        }

        public int Count
        {
            get
            {
                return _inner.Count;
            }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get
            {
                return _inner.Keys;
            }
        }

        public Dictionary<TKey, TValue>.ValueCollection Values
        {
            get
            {
                return _inner.Values;
            }
        }

        public void Add(TKey key, TValue value)
        {
            _inner.Add(key, value);

            if (dictionaryAdd != null) dictionaryAdd.OnNext(new DictionaryAddEvent<TKey, TValue>(key, value));
            if (countChanged != null) countChanged.OnNext(Count);
        }

        public void Clear()
        {
            var beforeCount = Count;
            _inner.Clear();

            if (collectionReset != null) collectionReset.OnNext(Unit.Default);
            if (beforeCount > 0)
            {
                if (countChanged != null) countChanged.OnNext(Count);
            }
        }

        public bool Remove(TKey key)
        {
            TValue oldValue;
            if (_inner.TryGetValue(key, out oldValue))
            {
                var isSuccessRemove = _inner.Remove(key);
                if (isSuccessRemove)
                {
                    if (dictionaryRemove != null) dictionaryRemove.OnNext(new DictionaryRemoveEvent<TKey, TValue>(key, oldValue));
                    if (countChanged != null) countChanged.OnNext(Count);
                }
                return isSuccessRemove;
            }
            else
            {
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _inner.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _inner.TryGetValue(key, out value);
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        void DisposeSubject<TSubject>(ref Subject<TSubject> subject)
        {
            if (subject != null)
            {
                try
                {
                    subject.OnCompleted();
                }
                finally
                {
                    subject.Dispose();
                    subject = null;
                }
            }
        }

        #region IDisposable Support

        public void Dispose() {
            if (_isDisposed)
                return;
            _isDisposed = true;
            DisposeSubject(ref countChanged);
            DisposeSubject(ref collectionReset);
            DisposeSubject(ref dictionaryAdd);
            DisposeSubject(ref dictionaryRemove);
            DisposeSubject(ref dictionaryReplace);
        }

        #endregion


        #region Observe

        Subject<int> countChanged = null;
        public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
        {
            if (_isDisposed) return Observable.Empty<int>();

            var subject = countChanged ?? (countChanged = new Subject<int>());
            if (notifyCurrentCount)
            {
                return Observable.Create<int>(observer => subject.StartWith(this.Count).Subscribe(observer));
            }
            else
            {
                return subject;
            }
        }

        Subject<Unit> collectionReset = null;
        public IObservable<Unit> ObserveReset()
        {
            if (_isDisposed) return Observable.Empty<Unit>();
            return collectionReset ?? (collectionReset = new Subject<Unit>());
        }

        Subject<DictionaryAddEvent<TKey, TValue>> dictionaryAdd = null;
        public IObservable<DictionaryAddEvent<TKey, TValue>> ObserveAdd()
        {
            if (_isDisposed) return Observable.Empty<DictionaryAddEvent<TKey, TValue>>();
            return dictionaryAdd ?? (dictionaryAdd = new Subject<DictionaryAddEvent<TKey, TValue>>());
        }

        Subject<DictionaryRemoveEvent<TKey, TValue>> dictionaryRemove = null;
        public IObservable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemove()
        {
            if (_isDisposed) return Observable.Empty<DictionaryRemoveEvent<TKey, TValue>>();
            return dictionaryRemove ?? (dictionaryRemove = new Subject<DictionaryRemoveEvent<TKey, TValue>>());
        }

        Subject<DictionaryReplaceEvent<TKey, TValue>> dictionaryReplace = null;
        public IObservable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplace()
        {
            if (_isDisposed) return Observable.Empty<DictionaryReplaceEvent<TKey, TValue>>();
            return dictionaryReplace ?? (dictionaryReplace = new Subject<DictionaryReplaceEvent<TKey, TValue>>());
        }

        #endregion

        #region implement explicit

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }

            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }


        bool IDictionary.IsFixedSize
        {
            get
            {
                return ((IDictionary)_inner).IsFixedSize;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return ((IDictionary)_inner).IsReadOnly;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((IDictionary)_inner).IsSynchronized;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return ((IDictionary)_inner).Keys;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((IDictionary)_inner).SyncRoot;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return ((IDictionary)_inner).Values;
            }
        }


        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return ((ICollection<KeyValuePair<TKey, TValue>>)_inner).IsReadOnly;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return _inner.Keys;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return _inner.Values;
            }
        }

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)_inner).Contains(key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)_inner).CopyTo(array, index);
        }

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add((TKey)item.Key, (TValue)item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_inner).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_inner).CopyTo(array, arrayIndex);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_inner).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue v;
            if (TryGetValue(item.Key, out v))
            {
                if (EqualityComparer<TValue>.Default.Equals(v, item.Value))
                {
                    Remove(item.Key);
                    return true;
                }
            }

            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_inner).GetEnumerator();
        }

        #endregion
    }
}
