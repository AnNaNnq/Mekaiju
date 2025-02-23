using Mekaiju.AI;
using MyBox;
using UnityEngine;


namespace Mekaiju.AI
{
    public class AbyssalVortex : IAttack
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
            Debug.Log($"Abyssal Vortex fait {damage} degats");

        }
    }
}
