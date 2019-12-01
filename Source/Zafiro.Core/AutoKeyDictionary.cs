namespace Zafiro.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class AutoKeyDictionary<TKey, TValue> : IEnumerable<TValue>, IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> dict;
        private readonly Func<TKey, TKey> getNextKey;
        private readonly Func<TKey, bool> isValidNextKey;

        public AutoKeyDictionary(Func<TKey, TKey> getNextKey, Func<TKey, bool> isValidNextKey) : this(getNextKey, isValidNextKey, new Dictionary<TKey, TValue>())
        {
        }

        public AutoKeyDictionary(Func<TKey, TKey> getNextKey, Func<TKey, bool> isValidNextKey, IDictionary<TKey, TValue> dictionary)
        {
            this.getNextKey = getNextKey;
            dict = dictionary;
            this.isValidNextKey = isValidNextKey;
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                var success = TryGetValue(key, out value);

                if (success)
                {
                    return value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }

            set
            {
                dict.Add(key, value);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dict.TryGetValue(key, out value) || TryResolveFromHierarchy(key, out value);
        }

        public void Add(TKey key, TValue value)
        {
            dict.Add(key, value);
        }

        private bool TryResolveFromHierarchy(TKey key, out TValue value)
        {
            bool success = false;
            value = default(TValue);

            while (isValidNextKey(key) && !success)
            {
                success = dict.TryGetValue(key, out value);
                key = getNextKey(key);
            }

            return success;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return dict.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            dict.Add(item);
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dict.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return dict.Remove(item);
        }

        public int Count => dict.Count;
        public bool IsReadOnly => dict.IsReadOnly;
        public bool ContainsKey(TKey key)
        {
            return dict.ContainsKey(key) || HierarchyContains(key);
        }

        private bool HierarchyContains(TKey key)
        {
            var success = false;

            while (isValidNextKey(key) && !success)
            {
                success = dict.ContainsKey(key);
                key = getNextKey(key);
            }

            return success;
        }

        public bool Remove(TKey key)
        {
            return dict.Remove(key);
        }

        public ICollection<TKey> Keys => dict.Keys;
        public ICollection<TValue> Values => dict.Values;
    }
}