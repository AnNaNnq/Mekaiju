using Mekaiju.AI.Attack.Instance;
using Mekaiju.Attribute;
using MyBox;
using UnityEngine;
using Mekaiju.Entity.Effect;
using Mekaiju.Entity;

namespace Mekaiju.AI.Attack
{
    public class RimeVoid : Attack
    {
        [Separator]
        [OverrideLabel("Rim prefab")][OpenPrefabButton] public GameObject gameObjectRimVoid;
        [OverrideLabel("Fire prefab")][OpenPrefabButton] public GameObject gameObjectRimVoidFire;
        [OverrideLabel("Duration (sec)")] public int rimVoidDuration = 2;
        [OverrideLabel("Hit cooldown (sec)")] public float rimVoidHitCooldown = 0.1f;
        [OverrideLabel("Modifier")][SOSelector] public Effect rimVoidEffect;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)p_kaiju;
            t_kaiju.motor.StopKaiju(1f);
            GameObject t_rim = GameObject.Instantiate(gameObjectRimVoid, p_kaiju.transform.position, Quaternion.identity);
            RimVoidInstance t_rv = t_rim.GetComponent<RimVoidInstance>();
            t_rv.SetUp(t_kaiju, this);
        }
    }
}
