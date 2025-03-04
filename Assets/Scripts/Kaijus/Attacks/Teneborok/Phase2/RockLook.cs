using MyBox;
using Unity.VisualScripting;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class RockLook : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        [OverrideLabel("Look Duration (sec)")]
        public float lookDuration = 2f;

        Transform _start;

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            _start = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;
            kaiju.motor.StopKaiju(lookDuration);
        }
    }
}
