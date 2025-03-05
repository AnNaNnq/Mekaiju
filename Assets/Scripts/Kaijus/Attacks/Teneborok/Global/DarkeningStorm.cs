using Mekaiju.Attribute;
using Mekaiju.Entity;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class DarkeningStorm : IAttack
    {
        [Separator]
        [OpenPrefabButton] public GameObject prefab;
        [OverrideLabel("Duration (sec)")] public float duration = 5f;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(IEntityInstance kaiju)
        {
            base.Active(kaiju);
            GameObject t_darkeningStorm = GameObject.Instantiate(prefab, kaiju.transform.position, Quaternion.identity);
            GameObject.Destroy(t_darkeningStorm, duration);
        }

        public override IEnumerator Attack(IEntityInstance kaiju)
        {
            base.Attack(kaiju);
            yield return new WaitForSeconds(duration);
        }
    }

}
