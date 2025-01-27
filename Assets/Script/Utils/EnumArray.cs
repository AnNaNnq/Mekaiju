using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mekaiju.Utils
{

    public class EnumArray 
    {

    }

    [Serializable]
    public class EnumArray<TKey, TValue> : EnumArray, IEnumerable<TValue> where TKey : Enum
    {
        [SerializeField]
        private TValue[] _array = new TValue[Enum.GetValues(typeof(TKey)).Length];

        public TValue this[TKey key]
        {
            get => _array[(int)(object)key];
            set => _array[(int)(object)key] = value;
        }

        public struct Enumerator : IEnumerator<TValue>
        {
            private TValue[] _array;
            private int      _index;

            public Enumerator(TValue[] p_array)
            {
                _array = p_array;
                _index = -1;
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _array.Length;
            }

            public TValue Current => _array[_index];
            object IEnumerator.Current => Current;

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose()
            {

            }
        }

        public IEnumerator<TValue> GetEnumerator() => new Enumerator(_array);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class EnumArrayExtensions
    {
        public static EnumArray<K, V> Select<F, K, V>(this EnumArray<K, F> self, Func<F, V> selector) where K: Enum
        {
            EnumArray<K, V> t_ret = new();
            foreach (var (i, v) in self.Select((v, i) => (i, v)))
            {
                t_ret[(K)Enum.ToObject(typeof(K), i)] = selector(v);
            }
            return t_ret;
        }
    }

}
