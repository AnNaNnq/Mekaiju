using System;
using System.Collections;
using System.Collections.Generic;
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

            public Enumerator(TValue[] t_array)
            {
                _array = t_array;
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

}
