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
        [OverrideLabel("Impact damage (% of DMG)")]
        public int damage = 10;
        [OverrideLabel("Fire tick damage (% of DMG)")]
        public float fireDamage;
        public float fireTickRate = 0.2f;
        [OverrideLabel("Prefab")][OpenPrefabButton] public GameObject doomsdayObject;
        [OpenPrefabButton] public GameObject fireZone;
        [HideInInspector] public Transform start;
        public int maxBounce = 5;
        public float speed = 10;

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

            t_druo.Init(this, kaiju);
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
