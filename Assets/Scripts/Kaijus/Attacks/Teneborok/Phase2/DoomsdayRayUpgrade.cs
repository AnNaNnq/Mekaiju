using Mekaiju.AI.Attack.Instance;
using Mekaiju.Attribute;
using Mekaiju.Entity;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class DoomsdayRayUpgrade : IAttack
    {
        [Separator]
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

        public override void Active(IEntityInstance kaiju)
        {
            base.Active(kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)kaiju;

            Transform start = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;

            GameObject t_doomsday = GameObject.Instantiate(doomsdayObject, start.position, Quaternion.identity);
            DoomsdayRayUpgradeObject t_druo = t_doomsday.GetComponent<DoomsdayRayUpgradeObject>();

            t_druo.Init(this, t_kaiju);
        }
    }
}
