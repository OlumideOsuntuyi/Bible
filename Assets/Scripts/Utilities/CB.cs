using System;
using System.Collections.Generic;

using LostOasis.Collections;

namespace LostOasis
{
    public static class CB
    {
        private static Dictionary<Type, object> BANK { get; set; }

        static CB()
        {
            BANK = new Dictionary<Type, object>();
        }

        public static void Push<T>(T item, int id)
        {
            Type type = typeof(T);
            if (!BANK.TryGetValue(type, out object result))
            {
                result = new ComponentBank<T>();
                BANK.Add(type, result);
            }

            ComponentBank<T> cb = result as ComponentBank<T>;
            cb.Push(item, id);
        }

        public static int Push<T>(T item)
        {
            Type type = typeof(T);
            if (!BANK.TryGetValue(type, out object result))
            {
                result = new ComponentBank<T>();
                BANK.Add(type, result);
            }

            ComponentBank<T> cb = result as ComponentBank<T>;
            return cb.Push(item);
        }

        public static ComponentBank<T> GetBank<T>()
        {
            Type type = typeof(T);
            if (!BANK.TryGetValue(type, out object result))
            {
                result = new ComponentBank<T>();
                BANK.Add(type, result);
            }

            return result as ComponentBank<T>;
        }

        public static int Count<T>()
        {
            if (BANK.TryGetValue(typeof(T), out object cb))
            {
                return (cb as ComponentBank<T>).Count;
            }
            return 0;
        }

        public static T Get<T>(int id)
        {
            return (BANK[typeof(T)] as ComponentBank<T>).Get(id);
        }

        public static T Get<T>(int id, T relative)
        {
            return (BANK[typeof(T)] as ComponentBank<T>).Get(id);
        }

        public static void Remove<T>(int id, T item)
        {
            (BANK[typeof(T)] as ComponentBank<T>).Remove(id);
        }

        public static void Remove<T>(int id)
        {
            (BANK[typeof(T)] as ComponentBank<T>).Remove(id);
        }
    }
}