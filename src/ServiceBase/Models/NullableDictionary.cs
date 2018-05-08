namespace ServiceBase.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Represents a dictionary which can hold null as valid key.
    /// </summary>
    /// <typeparam name="K">
    /// The type of the key elements.
    /// </typeparam>
    /// <typeparam name="V">
    /// The type of the value elements.
    /// </typeparam>
    public class NullableDictionary<K, V> : IDictionary<K, V>
    {
        #region Private Fields

        /// <summary>
        /// Saves the underlying dictionary instance.
        /// </summary>
        private Dictionary<K, V> _dict = new Dictionary<K, V>();

        /// <summary>
        /// Saves the value for the null key.
        /// </summary>
        private V _nullValue = default(V);

        /// <summary>
        /// Saves a flag indicating whether a value is set for the null key.
        /// </summary>
        private bool _hasNull = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new empty instance of the <see
        /// cref="NullableDictionary{K, V}" /> class.
        /// </summary>
        public NullableDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableDictionary{K,
        /// V}" /> class using the passed dictionary as seed.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary which should be used for initial values.
        /// </param>
        public NullableDictionary(IDictionary<K, V> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            foreach (KeyValuePair<K, V> pair in dictionary)
            {
                this[pair.Key] = pair.Value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the amount of elements in the current dictionary instance.
        /// </summary>
        public int Count
        {
            get
            {
                return this._dict.Count + (this._hasNull ? 1 : 0);
            }
        }

        /// <summary>
        /// Gets a flag indicating whether this is a read only dictionary.
        /// </summary>
        /// <remarks>
        /// Always false.
        /// </remarks>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a readonly collection of all keys in the current dictionary.
        /// </summary>
        public ICollection<K> Keys
        {
            get
            {
                if (!this._hasNull)
                {
                    return this._dict.Keys;
                }

                List<K> keys = this._dict.Keys.ToList();
                keys.Add(default(K));

                return new ReadOnlyCollection<K>(keys);
            }
        }

        /// <summary>
        /// Gets a readonly collection of all values in the current
        /// dictionary.
        /// </summary>
        public ICollection<V> Values
        {
            get
            {
                if (!this._hasNull)
                {
                    return this._dict.Values;
                }

                List<V> values = this._dict.Values.ToList();
                values.Add(this._nullValue);

                return new ReadOnlyCollection<V>(values);
            }
        }

        /// <summary>
        /// Gets or sets a value for a specific key.
        /// </summary>
        /// <exception cref="KeyNotFoundException">
        /// No element was found for the specified key.
        /// </exception>
        /// <param name="key">
        /// The key for which a value should be get or set. (Can be null)
        /// </param>
        /// <returns>
        /// The value for the specified key.
        /// </returns>
        public V this[K key]
        {
            get
            {
                if (key == null)
                {
                    if (this._hasNull)
                    {
                        return this._nullValue;
                    }
                    else
                    {
                        throw new KeyNotFoundException();
                    }
                }
                else
                {
                    return this._dict[key];
                }
            }

            set
            {
                if (key == null)
                {
                    this._nullValue = value;
                    this._hasNull = true;
                }
                else
                {
                    this._dict[key] = value;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a key value pair to the current dictionary.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// The key for which the value should be added already exists.
        /// </exception>
        /// <param name="key">
        /// The key for which the value should be set.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Add(K key, V value)
        {
            if (key == null)
            {
                if (this._hasNull)
                {
                    throw new ArgumentException("Duplicate key");
                }
                else
                {
                    this._nullValue = value;
                    this._hasNull = true;
                }
            }
            else
            {
                this._dict.Add(key, value);
            }
        }

        /// <summary>
        /// Checks whether the current dictionary contains the passed key.
        /// </summary>
        /// <param name="key">
        /// The key for which the dictionary should determine if it already
        /// exists.
        /// </param>
        /// <returns>
        /// A flag indicating whether the passed key is deposited in the
        /// current dictionary.
        /// </returns>
        public bool ContainsKey(K key)
        {
            if (key == null)
            {
                return this._hasNull;
            }

            return this._dict.ContainsKey(key);
        }

        /// <summary>
        /// Removes the passed key from the current dictionary.
        /// </summary>
        /// <param name="key">
        /// The key which should be removed.
        /// </param>
        /// <returns>
        /// A flag indicating whether the key was removed. (or didnt even
        /// exist = false)
        /// </returns>
        public bool Remove(K key)
        {
            if (key != null)
            {
                return this._dict.Remove(key);
            }

            bool oldHasNull = this._hasNull;
            this._hasNull = false;

            return oldHasNull;
        }

        /// <summary>
        /// Tries to get a value for the specified key, otherwise it sets the
        /// default value and returns false.
        /// </summary>
        /// <param name="key">
        /// The key for which a value should be retrieved.
        /// </param>
        /// <param name="value">
        /// The output parameter where the value should be put.
        /// </param>
        /// <returns>
        /// A flag indicating whether a value was retrieved.
        /// </returns>
        public bool TryGetValue(K key, out V value)
        {
            if (key != null)
            {
                return this._dict.TryGetValue(key, out value);
            }

            value = this._hasNull ? this._nullValue : default(V);

            return this._hasNull;
        }

        /// <summary>
        /// Adds a key value pair to the current dictionary.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// The key for which the value should be added already exists.
        /// </exception>
        /// <param name="item">
        /// The pair which should be added.
        /// </param>
        public void Add(KeyValuePair<K, V> item)
        {
            this.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes all keys from the current dictionary.
        /// </summary>
        public void Clear()
        {
            this._hasNull = false;
            this._dict.Clear();
        }

        /// <summary>
        /// Checks whether the current dictionary contains the passed pair.
        /// </summary>
        /// <param name="item">
        /// The pair for which the dictionary should determine if its
        /// included.
        /// </param>
        /// <returns>
        /// A flag indicating whether the current dictionary contains the
        /// passed pair.
        /// </returns>
        public bool Contains(KeyValuePair<K, V> item)
        {
            if (item.Key != null)
            {
                return ((ICollection<KeyValuePair<K, V>>)this._dict)
                    .Contains(item);
            }

            if (this._hasNull)
            {
                return EqualityComparer<V>.Default.Equals(
                    this._nullValue, item.Value);
            }

            return false;
        }

        /// <summary>
        /// Copies all pairs of the current dictionary into the passed array
        /// at the specified index.
        /// </summary>
        /// <param name="array">
        /// The target array into which the current dictionary should be
        /// copied.
        /// </param>
        /// <param name="arrayIndex">
        /// The offset in the array starting from which the elements should be
        /// placed.
        /// </param>
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<K, V>>)this._dict)
                .CopyTo(array, arrayIndex);

            if (this._hasNull)
            {
                array[arrayIndex + this._dict.Count] = new KeyValuePair<K, V>(
                    default(K), this._nullValue);
            }
        }

        /// <summary>
        /// Removes the passed pair from the current dictionary.
        /// </summary>
        /// <param name="item">
        /// The pair which should be removed.
        /// </param>
        /// <returns>
        /// A flag indicating whether the passed pair was removed.
        /// </returns>
        public bool Remove(KeyValuePair<K, V> item)
        {
            V value;

            if (this.TryGetValue(item.Key, out value) &&
                EqualityComparer<V>.Default.Equals(item.Value, value))
            {
                return Remove(item.Key);
            }

            return false;
        }

        /// <summary>
        /// Gets a enumerator for the current dictionary.
        /// </summary>
        /// <returns>
        /// A enumerator instance for the current dictionary.
        /// </returns>
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            if (!this._hasNull)
            {
                return this._dict.GetEnumerator();
            }
            else
            {
                return this.GetEnumeratorWithNull();
            }
        }

        /// <summary>
        /// Gets a enumerator for the current dictionary.
        /// </summary>
        /// <returns>
        /// A enumerator instance for the current dictionary.
        /// </returns>
        System.Collections.IEnumerator
            System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a enumerator for the current dictionary including the null
        /// value.
        /// </summary>
        /// <returns>
        /// A enumerator instance for the current dictionary.
        /// </returns>
        private IEnumerator<KeyValuePair<K, V>> GetEnumeratorWithNull()
        {
            yield return new KeyValuePair<K, V>(default(K), this._nullValue);

            foreach (KeyValuePair<K, V> kv in this._dict)
            {
                yield return kv;
            }
        }

        #endregion
    }
}
