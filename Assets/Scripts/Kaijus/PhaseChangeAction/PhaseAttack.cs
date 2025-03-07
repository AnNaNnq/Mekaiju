using Mekaiju.Attribute;
using Mekaiju.QTE;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI.PhaseAttack
{
    [System.Serializable]
    public abstract class PhaseAttack
    {
        public bool canMakeDamage = true;

        [ConditionalField(nameof(canMakeDamage))]
        [Indent]
        public float damage = 50;
        [ConditionalField(nameof(canMakeDamage))]
        [Indent]
        public bool blockable;
        [Header("QTE")]
        public float time;
        public float quantity;

        protected QTESystem _qte;
        protected KaijuInstance _kaiju;
        public int input { get; protected set; }

        public void Init(KaijuInstance p_kaiju)
        {
            _kaiju = p_kaiju;
            _qte = new QTESystem(this);
        }

        public virtual void Action() { }

        public virtual void Success() { }

        public virtual void Failure() { }
    }
}
