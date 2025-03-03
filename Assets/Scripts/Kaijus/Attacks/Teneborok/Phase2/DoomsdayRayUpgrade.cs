using Mekaiju.AI.Attack.Instance;
using Mekaiju.Attribute;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class DoomsdayRayUpgrade : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public int damage = 10;
        [OverrideLabel("Prefab")][OpenPrefabButton] public GameObject doomsdayObject;
        [HideInInspector] public Transform start;
        public int maxBounce = 5;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            kaiju.StartCoroutine(Attack(kaiju));

            Transform start = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;

            GameObject t_doomsday = GameObject.Instantiate(doomsdayObject, start.position, Quaternion.identity);
            DoomsdayRayUpgradeObject t_druo = t_doomsday.GetComponent<DoomsdayRayUpgradeObject>();

            t_druo.Init(this);
        }

        public override IEnumerator Attack(KaijuInstance kaiju)
        {
            base.Attack(kaiju);
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                
            }
        }
    }
}
