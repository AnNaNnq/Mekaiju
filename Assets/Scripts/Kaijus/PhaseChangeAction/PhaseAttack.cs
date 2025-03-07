using Mekaiju.Attribute;
using MyBox;

namespace Mekaiju.AI.PhaseAttack
{
    [System.Serializable]
    public abstract class PhaseAttack
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
