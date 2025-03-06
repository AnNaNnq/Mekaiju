using Mekaiju.Attribute;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI.PhaseAttack
{
    [System.Serializable]
    public abstract class IPhaseAttack
    {
        protected KaijuInstance _kaiju;

        public bool canMakeDamage = true;

        [ConditionalField(nameof(canMakeDamage))]
        [Indent]
        public float damage = 50;
        [ConditionalField(nameof(canMakeDamage))]
        [Indent]
        public bool blockable;

        public void Init(KaijuInstance p_kaiju)
        {
            _kaiju = p_kaiju;
        }

        public virtual void Action() { }
    }
}
