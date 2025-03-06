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

        public override void Active(IEntityInstance p_kaiju)
        {
            base.Active(p_kaiju);
            GameObject t_darkeningStorm = GameObject.Instantiate(prefab, p_kaiju.transform.position, Quaternion.identity);
            GameObject.Destroy(t_darkeningStorm, duration);
        }

        public override IEnumerator Attack(IEntityInstance p_kaiju)
        {
            base.Attack(p_kaiju);
            yield return new WaitForSeconds(duration);
        }
    }

}
