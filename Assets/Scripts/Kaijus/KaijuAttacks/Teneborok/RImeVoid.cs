using MyBox;
using UnityEngine;

namespace Mekaiju.AI
{
    public class RimeVoid : IAttack
    {
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active()
        {
            base.Active();
            Debug.Log($"Rime Void fait {damage} degats");

        }
    }
}
