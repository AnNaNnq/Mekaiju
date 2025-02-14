using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mekaiju
{
    public enum ModifierTarget
    {
        Speed, Defense, Damage
    }

    [Serializable]
    public class Modifier
    {
        public float value;

        public Modifier(float p_value)
        {
            value = p_value;
        }
    }

    [Serializable]
    public class ModifierCollection
    {
        private List<Modifier> _values;

        public ModifierCollection()
        {
            _values = new();
        }

        public Modifier Add(float value)
        {
            _values.Add(new(value));
            return _values[^1];
        }

        public void Remove(Modifier p_modifier)
        {
            _values.Remove(p_modifier);
        }

        public float ComputeValue(float p_baseValue)
        {
            return p_baseValue + _values.Aggregate(0f, (t_acc, t_mod) => t_acc + (t_mod.value * p_baseValue));
        }    
    }
}
