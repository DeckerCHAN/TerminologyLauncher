using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace TerminologyLauncher.GUI.Toolkits
{
    [Serializable]
    public sealed class ObservableDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IDictionary,
        ICollection,
        IEnumerable,
        ISerializable,
        IDeserializationCallback,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        #region constructors

        #region public

        public ObservableDictionary()
        {
            this._keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this._keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();

            foreach (KeyValuePair<TKey, TValue> entry in dictionary)
                this.DoAddEntry((TKey) entry.Key, (TValue) entry.Value);
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            this._keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this._keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);

            foreach (KeyValuePair<TKey, TValue> entry in dictionary)
                this.DoAddEntry((TKey) entry.Key, (TValue) entry.Value);
        }

        #endregion public

        #region protected

        private ObservableDictionary(SerializationInfo info, StreamingContext context)
        {
            this._siInfo = info;
        }

        #endregion protected

        #endregion constructors

        #region properties

        #region public

        public IEqualityComparer<TKey> Comparer => this._keyedEntryCollection.Comparer;

        public int Count => this._keyedEntryCollection.Count;

        public Dictionary<TKey, TValue>.KeyCollection Keys => this.TrueDictionary.Keys;

        public TValue this[TKey key]
        {
            get { return (TValue) this._keyedEntryCollection[key].Value; }
            set { this.DoSetEntry(key, value); }
        }

        public Dictionary<TKey, TValue>.ValueCollection Values => this.TrueDictionary.Values;

        #endregion public

        #region private

        private Dictionary<TKey, TValue> TrueDictionary
        {
            get
            {
                if (this._dictionaryCacheVersion != this._version)
                {
                    this._dictionaryCache.Clear();
                    foreach (DictionaryEntry entry in this._keyedEntryCollection)
                        this._dictionaryCache.Add((TKey) entry.Key, (TValue) entry.Value);
                    this._dictionaryCacheVersion = this._version;
                }
                return this._dictionaryCache;
            }
        }

        #endregion private

        #endregion properties

        #region methods

        #region public

        public void Add(TKey key, TValue value)
        {
            this.DoAddEntry(key, value);
        }

        public void Clear()
        {
            this.DoClearEntries();
        }

        public bool ContainsKey(TKey key)
        {
            return this._keyedEntryCollection.Contains(key);
        }

        public bool ContainsValue(TValue value)
        {
            return this.TrueDictionary.ContainsValue(value);
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator<TKey, TValue>(this, false);
        }

        public bool Remove(TKey key)
        {
            return this.DoRemoveEntry(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool result = this._keyedEntryCollection.Contains(key);
            value = result ? (TValue) this._keyedEntryCollection[key].Value : default(TValue);
            return result;
        }

        #endregion public

        #region protected

        private bool AddEntry(TKey key, TValue value)
        {
            this._keyedEntryCollection.Add(new DictionaryEntry(key, value));
            return true;
        }

        private bool ClearEntries()
        {
            // check whether there are entries to clear
            bool result = (this.Count > 0);
            if (result)
            {
                // if so, clear the dictionary
                this._keyedEntryCollection.Clear();
            }
            return result;
        }

        private int GetIndexAndEntryForKey(TKey key, out DictionaryEntry entry)
        {
            entry = new DictionaryEntry();
            int index = -1;
            if (this._keyedEntryCollection.Contains(key))
            {
                entry = this._keyedEntryCollection[key];
                index = this._keyedEntryCollection.IndexOf(entry);
            }
            return index;
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, args);
        }

        private void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private bool RemoveEntry(TKey key)
        {
            // remove the entry
            return this._keyedEntryCollection.Remove(key);
        }

        private bool SetEntry(TKey key, TValue value)
        {
            bool keyExists = this._keyedEntryCollection.Contains(key);

            // if identical key/value pair already exists, nothing to do
            if (keyExists && value.Equals((TValue) this._keyedEntryCollection[key].Value))
                return false;

            // otherwise, remove the existing entry
            if (keyExists)
                this._keyedEntryCollection.Remove(key);

            // add the new entry
            this._keyedEntryCollection.Add(new DictionaryEntry(key, value));

            return true;
        }

        #endregion protected

        #region private

        private void DoAddEntry(TKey key, TValue value)
        {
            if (this.AddEntry(key, value))
            {
                this._version++;

                DictionaryEntry entry;
                int index = this.GetIndexAndEntryForKey(key, out entry);
                this.FireEntryAddedNotifications(entry, index);
            }
        }

        private void DoClearEntries()
        {
            if (this.ClearEntries())
            {
                this._version++;
                this.FireResetNotifications();
            }
        }

        private bool DoRemoveEntry(TKey key)
        {
            DictionaryEntry entry;
            int index = this.GetIndexAndEntryForKey(key, out entry);

            bool result = this.RemoveEntry(key);
            if (result)
            {
                this._version++;
                if (index > -1)
                    this.FireEntryRemovedNotifications(entry, index);
            }

            return result;
        }

        private void DoSetEntry(TKey key, TValue value)
        {
            DictionaryEntry entry;
            int index = this.GetIndexAndEntryForKey(key, out entry);

            if (this.SetEntry(key, value))
            {
                this._version++;

                // if prior entry existed for this key, fire the removed notifications
                if (index > -1)
                {
                    this.FireEntryRemovedNotifications(entry, index);

                    // force the property change notifications to fire for the modified entry
                    this._countCache--;
                }

                // then fire the added notifications
                index = this.GetIndexAndEntryForKey(key, out entry);
                this.FireEntryAddedNotifications(entry, index);
            }
        }

        private void FireEntryAddedNotifications(DictionaryEntry entry, int index)
        {
            // fire the relevant PropertyChanged notifications
            this.FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            if (index > -1)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    new KeyValuePair<TKey, TValue>((TKey) entry.Key, (TValue) entry.Value), index));
            else
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void FireEntryRemovedNotifications(DictionaryEntry entry, int index)
        {
            // fire the relevant PropertyChanged notifications
            this.FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            if (index > -1)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                    new KeyValuePair<TKey, TValue>((TKey) entry.Key, (TValue) entry.Value), index));
            else
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void FirePropertyChangedNotifications()
        {
            if (this.Count != this._countCache)
            {
                this._countCache = this.Count;
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnPropertyChanged("Keys");
                this.OnPropertyChanged("Values");
            }
        }

        private void FireResetNotifications()
        {
            // fire the relevant PropertyChanged notifications
            this.FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion private

        #endregion methods

        #region interfaces

        #region IDictionary<TKey, TValue>

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            this.DoAddEntry(key, value);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return this.DoRemoveEntry(key);
        }

        bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
        {
            return this._keyedEntryCollection.Contains(key);
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return this.TryGetValue(key, out value);
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => this.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => this.Values;

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get { return (TValue) this._keyedEntryCollection[key].Value; }
            set { this.DoSetEntry(key, value); }
        }

        #endregion IDictionary<TKey, TValue>

        #region IDictionary

        void IDictionary.Add(object key, object value)
        {
            this.DoAddEntry((TKey) key, (TValue) value);
        }

        void IDictionary.Clear()
        {
            this.DoClearEntries();
        }

        bool IDictionary.Contains(object key)
        {
            return this._keyedEntryCollection.Contains((TKey) key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator<TKey, TValue>(this, true);
        }

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        object IDictionary.this[object key]
        {
            get { return this._keyedEntryCollection[(TKey) key].Value; }
            set { this.DoSetEntry((TKey) key, (TValue) value); }
        }

        ICollection IDictionary.Keys => this.Keys;

        void IDictionary.Remove(object key)
        {
            this.DoRemoveEntry((TKey) key);
        }

        ICollection IDictionary.Values => this.Values;

        #endregion IDictionary

        #region ICollection<KeyValuePair<TKey, TValue>>

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> kvp)
        {
            this.DoAddEntry(kvp.Key, kvp.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            this.DoClearEntries();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> kvp)
        {
            return this._keyedEntryCollection.Contains(kvp.Key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("CopyTo() failed:  array parameter was null");
            }
            if ((index < 0) || (index > array.Length))
            {
                throw new ArgumentOutOfRangeException(
                    "CopyTo() failed:  index parameter was outside the bounds of the supplied array");
            }
            if ((array.Length - index) < this._keyedEntryCollection.Count)
            {
                throw new ArgumentException("CopyTo() failed:  supplied array was too small");
            }

            foreach (DictionaryEntry entry in this._keyedEntryCollection)
                array[index++] = new KeyValuePair<TKey, TValue>((TKey) entry.Key, (TValue) entry.Value);
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count => this._keyedEntryCollection.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> kvp)
        {
            return this.DoRemoveEntry(kvp.Key);
        }

        #endregion ICollection<KeyValuePair<TKey, TValue>>

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection) this._keyedEntryCollection).CopyTo(array, index);
        }

        int ICollection.Count => this._keyedEntryCollection.Count;

        bool ICollection.IsSynchronized => ((ICollection) this._keyedEntryCollection).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection) this._keyedEntryCollection).SyncRoot;

        #endregion ICollection

        #region IEnumerable<KeyValuePair<TKey, TValue>>

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator<TKey, TValue>(this, false);
        }

        #endregion IEnumerable<KeyValuePair<TKey, TValue>>

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable

        #region ISerializable

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Collection<DictionaryEntry> entries = new Collection<DictionaryEntry>();
            foreach (DictionaryEntry entry in this._keyedEntryCollection)
                entries.Add(entry);
            info.AddValue("entries", entries);
        }

        #endregion ISerializable

        #region IDeserializationCallback

        public void OnDeserialization(object sender)
        {
            if (this._siInfo != null)
            {
                Collection<DictionaryEntry> entries = (Collection<DictionaryEntry>)
                    this._siInfo.GetValue("entries", typeof(Collection<DictionaryEntry>));
                foreach (DictionaryEntry entry in entries)
                    this.AddEntry((TKey) entry.Key, (TValue) entry.Value);
            }
        }

        #endregion IDeserializationCallback

        #region INotifyCollectionChanged

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { this.CollectionChanged += value; }
            remove { this.CollectionChanged -= value; }
        }

        private event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion INotifyCollectionChanged

        #region INotifyPropertyChanged

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this.PropertyChanged += value; }
            remove { this.PropertyChanged -= value; }
        }

        private event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #endregion interfaces

        #region protected classes

        #region KeyedDictionaryEntryCollection<TKey>

        private class KeyedDictionaryEntryCollection<TIsKey> : KeyedCollection<TIsKey, DictionaryEntry>
        {
            #region constructors

            #region public

            public KeyedDictionaryEntryCollection() : base()
            {
            }

            public KeyedDictionaryEntryCollection(IEqualityComparer<TIsKey> comparer) : base(comparer)
            {
            }

            #endregion public

            #endregion constructors

            #region methods

            #region protected

            protected override TIsKey GetKeyForItem(DictionaryEntry entry)
            {
                return (TIsKey) entry.Key;
            }

            #endregion protected

            #endregion methods
        }

        #endregion KeyedDictionaryEntryCollection<TKey>

        #endregion protected classes

        #region public structures

        #region Enumerator

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator<TIsKey, TIsValue> : IEnumerator<KeyValuePair<TIsKey, TIsValue>>, IDisposable,
            IDictionaryEnumerator, IEnumerator
        {
            #region constructors

            internal Enumerator(ObservableDictionary<TIsKey, TIsValue> dictionary, bool isDictionaryEntryEnumerator)
            {
                this._dictionary = dictionary;
                this._version = dictionary._version;
                this._index = -1;
                this._isDictionaryEntryEnumerator = isDictionaryEntryEnumerator;
                this._current = new KeyValuePair<TIsKey, TIsValue>();
            }

            #endregion constructors

            #region properties

            #region public

            public KeyValuePair<TIsKey, TIsValue> Current
            {
                get
                {
                    this.ValidateCurrent();
                    return this._current;
                }
            }

            #endregion public

            #endregion properties

            #region methods

            #region public

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                this.ValidateVersion();
                this._index++;
                if (this._index < this._dictionary._keyedEntryCollection.Count)
                {
                    this._current =
                        new KeyValuePair<TIsKey, TIsValue>(
                            (TIsKey) this._dictionary._keyedEntryCollection[this._index].Key,
                            (TIsValue) this._dictionary._keyedEntryCollection[this._index].Value);
                    return true;
                }
                this._index = -2;
                this._current = new KeyValuePair<TIsKey, TIsValue>();
                return false;
            }

            #endregion public

            #region private

            private void ValidateCurrent()
            {
                if (this._index == -1)
                {
                    throw new InvalidOperationException("The enumerator has not been started.");
                }
                else if (this._index == -2)
                {
                    throw new InvalidOperationException("The enumerator has reached the end of the collection.");
                }
            }

            private void ValidateVersion()
            {
                if (this._version != this._dictionary._version)
                {
                    throw new InvalidOperationException("The enumerator is not valid because the dictionary changed.");
                }
            }

            #endregion private

            #endregion methods

            #region IEnumerator implementation

            object IEnumerator.Current
            {
                get
                {
                    this.ValidateCurrent();
                    if (this._isDictionaryEntryEnumerator)
                    {
                        return new DictionaryEntry(this._current.Key, this._current.Value);
                    }
                    return new KeyValuePair<TIsKey, TIsValue>(this._current.Key, this._current.Value);
                }
            }

            void IEnumerator.Reset()
            {
                this.ValidateVersion();
                this._index = -1;
                this._current = new KeyValuePair<TIsKey, TIsValue>();
            }

            #endregion IEnumerator implemenation

            #region IDictionaryEnumerator implemenation

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    this.ValidateCurrent();
                    return new DictionaryEntry(this._current.Key, this._current.Value);
                }
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    this.ValidateCurrent();
                    return this._current.Key;
                }
            }

            object IDictionaryEnumerator.Value
            {
                get
                {
                    this.ValidateCurrent();
                    return this._current.Value;
                }
            }

            #endregion

            #region fields

            private ObservableDictionary<TIsKey, TIsValue> _dictionary;
            private int _version;
            private int _index;
            private KeyValuePair<TIsKey, TIsValue> _current;
            private bool _isDictionaryEntryEnumerator;

            #endregion fields
        }

        #endregion Enumerator

        #endregion public structures

        #region fields

        private KeyedDictionaryEntryCollection<TKey> _keyedEntryCollection;

        private int _countCache = 0;
        private Dictionary<TKey, TValue> _dictionaryCache = new Dictionary<TKey, TValue>();
        private int _dictionaryCacheVersion = 0;
        private int _version = 0;

        [NonSerialized] private SerializationInfo _siInfo = null;

        #endregion fields
    }
}