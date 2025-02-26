using System.Collections;
using UnityEngine;

namespace Mekaiju.AI {
    [System.Serializable]
    public abstract class IAttack
    {
        public float cooldown;
        public float range;
        public bool blockable;

        [HideInInspector]
        public bool canUse;

        public IAttack()
        {
            canUse = true;
        }

        public virtual bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            bool t_return = canUse && kaiju.TargetInRange(range);
            if (otherRange > 0)
            {
                t_return &= !kaiju.TargetInRange(otherRange);
            }
            return t_return;
        }

        public virtual void Active(KaijuInstance kaiju) { }

        public virtual IEnumerator Attack(KaijuInstance kaiju)
        {
            yield return null;
        }
    }
}
