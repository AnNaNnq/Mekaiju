using MyBox;
using UnityEngine;

namespace Mekaiju.AI
{
    public class SharpBlow : IAttack
    {
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;

        public override void Active()
        {
            base.Active();
            
        }
    }
}
