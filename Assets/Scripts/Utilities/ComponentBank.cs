using System;
using System.Collections.Generic;

namespace LostOasis.Collections
{
    public sealed class ComponentBank<T>
    {
        Random random;
        public Dictionary<int, T> bank { get; private set; }

        public ComponentBank()
        {
            random = new Random(DateTime.Now.Millisecond ^ DateTime.Now.Second);
            bank = new();
        }

        public T this[int ID]
        {
            get
            {
                return Get(ID);
            }
        }

        private int ID()
        {
            int id;
            do
            {
                id = random.Next(int.MinValue, int.MaxValue);
            }
            while (Contains(id) || id == 0);
            return id;
        }

        /// <summary>
        /// Adds object to bank with RANDOM key
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Object ID in BANK</returns>
        public int Push(T value)
        {
            int id = ID();
            bank.Add(id, value);
            return id;
        }

        /// <summary>
        /// Add with UNIQUE ID
        /// </summary>
        public void Push(T value, int ID)
        {
            bank.Add(ID, value);
        }
        public bool ForceAddMain(T value, int id)
        {
            if (Contains(id))
                return false;

            bank.Add(id, value);
            return true;
        }
        public void Remove(int id)
        {
            bank.Remove(id);
        }
        public T Get(int id)
        {
            _ = bank.TryGetValue(id, out T result);
            return result;
        }
        public bool Contains(int id)
        {
            return bank.ContainsKey(id);
        }
        public int Count => bank.Count;
        public void ForEach(Action<T> action)
        {
            foreach (var value in bank.Values)
            {
                action.Invoke(value);
            }
        }
    }
}