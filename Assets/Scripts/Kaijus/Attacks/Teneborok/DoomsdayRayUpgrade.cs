using Mekaiju.AI.Attack.Instance;
using Mekaiju.Attribute;
using Mekaiju.Entity;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class DoomsdayRayUpgrade : Attack
    {
        [Separator]
        public DamageZoneStats fireZone;
        [OverrideLabel("Prefab")][OpenPrefabButton] public GameObject doomsdayObject;
        [OpenPrefabButton] public GameObject fireZonePrefab;
        [HideInInspector] public Transform start;
        public int maxBounce = 5;
        public float speed = 10;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)p_kaiju;

            Transform t_start = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;

            GameObject t_doomsday = GameObject.Instantiate(doomsdayObject, t_start.position, Quaternion.identity);
            DoomsdayRayUpgradeObject t_druo = t_doomsday.GetComponent<DoomsdayRayUpgradeObject>();

            t_druo.Init(this, t_kaiju);
            OnEnd();
        }
    }
}
