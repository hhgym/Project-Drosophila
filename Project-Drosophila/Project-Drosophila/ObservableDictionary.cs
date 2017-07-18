using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Project_Drosophila
{
    public class ObservableDictionary<TKey, TValue>
        : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        readonly IDictionary<TKey, TValue> dictionary;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public int Count => dictionary.Count;

        public ObservableDictionary() : this(new Dictionary<TKey, TValue>())
        {
        }

        /// <summary>
        /// Initializes an instance of the class using another dictionary as 
        /// the key/value store.
        /// </summary>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        void Update(TKey key, TValue value)
        {
            TValue existing;
            if (dictionary.TryGetValue(key, out existing))
            {
                dictionary[key] = value;

                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                    new KeyValuePair<TKey, TValue>(key, value),
                    new KeyValuePair<TKey, TValue>(key, existing)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
            }
            else
                Add(key, value);
        }
        
        protected void RaisePropertyChanged(PropertyChangedEventArgs args)
            => PropertyChanged?.Invoke(this, args);

        #region IDictionary<TKey,TValue> Members
        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                new KeyValuePair<TKey, TValue>(key, value)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Keys"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
        }
        
        public bool ContainsKey(TKey key)
            => dictionary.ContainsKey(key);
        
        public ICollection<TKey> Keys
            => dictionary.Keys;
        
        public bool Remove(TKey key)
        {
            ICollection<TKey> keys = Keys;
            int i = 0;
            int index = -1;
            foreach (TKey savedKey in keys)
            {
                if (savedKey.Equals(key))
                {
                    index = i;
                    break;
                }
                ++i;
            }
            if (index == -1)
                return false;

            TValue value = dictionary[key];
            if (!dictionary.Remove(key))
                return false;

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                new KeyValuePair<TKey, TValue>(key, value), index));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Keys"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
            => dictionary.TryGetValue(key, out value);

        public ICollection<TValue> Values
            => dictionary.Values;

        public TValue this[TKey key]
        {
            get { return dictionary[key]; }
            set { Update(key, value); }
        }
        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            => Add(item.Key, item.Value);

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            dictionary.Clear();

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Keys"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
            => dictionary.Contains(item);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => dictionary.CopyTo(array, arrayIndex);

        int ICollection<KeyValuePair<TKey, TValue>>.Count
            => dictionary.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
            => dictionary.IsReadOnly;

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            => Remove(item.Key);
        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => dictionary.GetEnumerator();
        #endregion
    }
}
