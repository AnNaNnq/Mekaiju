using System;
using System.Collections.Generic;
using System.Linq;

namespace Mekaiju.Entity
{
    [Serializable]
    public enum ModifierKind
    {
        Flat, Percent
    }

    [Serializable]
    public class Modifier
    {
        public float        value;
        public ModifierKind kind;

        public Modifier(float p_value, ModifierKind p_kind)
        {
            value = p_value;
            kind  = p_kind;
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

        public Modifier Add(float p_value, ModifierKind p_kind)
        {
            _values.Add(new(p_value, p_kind));
            return _values[^1];
        }

        public void Remove(Modifier p_modifier)
        {
            _values.Remove(p_modifier);
        }

        public float ComputeValue(float p_baseValue)
        {
            return p_baseValue + _values.Aggregate(
                0f, 
                (t_acc, t_mod) => t_acc + (t_mod.kind == ModifierKind.Flat ? t_mod.value : (t_mod.value * p_baseValue))
            );
        }
    }
}
