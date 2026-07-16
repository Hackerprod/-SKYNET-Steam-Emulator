using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SKYNET.Helpers
{
    /// <summary>
    /// Thread-safe replacement for List<T>; backed by a ConcurrentDictionary.
    /// - Lock-free reads and writes for the caller.
    /// - O(1) access by key via GetByKey/TryGet (vs O(n) List.Find).
    /// - Keeps the List<T>; member names so most call-sites compile unchanged.
    ///
    /// Differences from List<T>; (read before migrating a collection):
    /// 1. Does NOT preserve insertion order. If a collection depends on order
    ///    (Sort, Insert, list[i], IndexOf, FIFO), keep it as List<T>; + a lock.
    /// 2. No duplicate keys: Add with an existing key REPLACES (upsert). Correct
    ///    for entities with a unique id; wrong where duplicates are expected.
    /// 3. The key must be stable: don't change an item's key after adding it.
    /// 4. Compound (multi-step) operations are still not atomic — hold a lock
    ///    around read-modify-write sequences that must be transactional.
    /// </summary>
    public class FastList<T> : IEnumerable<T>
    {
        private readonly ConcurrentDictionary<object, T> Items;
        private readonly Func<T, object> KeySelector;

        /// <summary>
        /// With a key selector: new FastList<Player>;(p =>; p.ID). Enables O(1)
        /// GetByKey/TryGet. Always look up with the SAME key type used to store
        /// (a boxed ulong and a boxed int never compare equal).
        /// </summary>
        public FastList(Func<T, object> keySelector)
        {
            KeySelector = keySelector;
            Items = new ConcurrentDictionary<object, T>();
        }

        /// <summary>
        /// No selector: the item itself is the key (set semantics via Equals/GetHashCode).
        /// </summary>
        public FastList() : this(item => (object)item)
        {
        }

        // ==================== List<T>-compatible surface ====================

        /// <summary>Add or replace (upsert) by key.</summary>
        public void Add(T item)
        {
            Items[KeySelector(item)] = item;
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (T item in items) Add(item);
        }

        public bool Remove(T item)
        {
            return Items.TryRemove(KeySelector(item), out _);
        }

        public int RemoveAll(Predicate<T> match)
        {
            int removed = 0;
            foreach (KeyValuePair<object, T> pair in Items)
            {
                if (match(pair.Value) && Items.TryRemove(pair.Key, out _))
                {
                    removed++;
                }
            }
            return removed;
        }

        /// <summary>O(n) like List.Find, but without blocking anyone.</summary>
        public T Find(Predicate<T> match)
        {
            foreach (KeyValuePair<object, T> pair in Items)
            {
                if (match(pair.Value)) return pair.Value;
            }
            return default(T);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            List<T> result = new List<T>();
            foreach (KeyValuePair<object, T> pair in Items)
            {
                if (match(pair.Value)) result.Add(pair.Value);
            }
            return result;
        }

        public bool Exists(Predicate<T> match)
        {
            foreach (KeyValuePair<object, T> pair in Items)
            {
                if (match(pair.Value)) return true;
            }
            return false;
        }

        public bool Contains(T item)
        {
            return Items.ContainsKey(KeySelector(item));
        }

        public void ForEach(Action<T> action)
        {
            foreach (KeyValuePair<object, T> pair in Items) action(pair.Value);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public int Count
        {
            get { return Items.Count; }
        }

        /// <summary>Cheaper than Count == 0 (Count takes internal locks).</summary>
        public bool IsEmpty
        {
            get { return Items.IsEmpty; }
        }

        public List<T> ToList()
        {
            List<T> list = new List<T>(Items.Count);
            foreach (KeyValuePair<object, T> pair in Items) list.Add(pair.Value);
            return list;
        }

        public T[] ToArray()
        {
            return ToList().ToArray();
        }

        // ============ O(1) access by key (the payoff) ============

        /// <summary>O(1). Returns default/null when absent, like Find.</summary>
        public T GetByKey(object key)
        {
            Items.TryGetValue(key, out T item);
            return item;
        }

        public bool TryGet(object key, out T item)
        {
            return Items.TryGetValue(key, out item);
        }

        public bool ContainsKey(object key)
        {
            return Items.ContainsKey(key);
        }

        public bool RemoveByKey(object key)
        {
            return Items.TryRemove(key, out _);
        }

        // ==================== foreach / LINQ ====================

        /// <summary>
        /// Lock-free enumeration that is safe even if another thread mutates the
        /// collection mid-iteration (unlike List, which throws
        /// InvalidOperationException). Enables LINQ (Where, Select, Any...).
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (KeyValuePair<object, T> pair in Items)
            {
                yield return pair.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
