using Mekaiju.AI.Attack;
using Mekaiju.Attribute;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI
{
    public class RimeVoid : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        [OverrideLabel("Rim prefab")][OpenPrefabButton] public GameObject gameObjectRimVoid;
        [OverrideLabel("Fire prefab")][OpenPrefabButton] public GameObject gameObjectRimVoidFire;
        [OverrideLabel("Duration (sec)")] public int rimVoidDuration = 2;
        [OverrideLabel("Hit cooldown (sec)")] public float rimVoidHitCooldown = 0.1f;
        [OverrideLabel("Modifier")][SOSelector] public Effect rimVoidEffect;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            kaiju.motor.StopKaiju(1f);
            GameObject t_rim = GameObject.Instantiate(gameObjectRimVoid, kaiju.transform.position, Quaternion.identity);
            RimVoidInstance t_rv = t_rim.GetComponent<RimVoidInstance>();
            t_rv.SetUp(kaiju, this);
        }
    }
}
