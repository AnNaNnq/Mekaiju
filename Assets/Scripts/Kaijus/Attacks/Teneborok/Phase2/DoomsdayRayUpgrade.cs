using Mekaiju.AI.Attack.Instance;
using Mekaiju.Attribute;
using Mekaiju.Utils;
using MyBox;
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
        [OverrideLabel("Ray Speed")] public float speed = 10f;
        [Range(1,2)]
        public float bounceDamping = 1.2f;
        public int nbBounce = 5;
        public float bounceForce = 10f;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            start = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;
            Vector3 t_pos = new Vector3(kaiju.transform.position.x, UtilsFunctions.GetGround(kaiju.transform.position), kaiju.transform.position.z) + (kaiju.transform.forward * 10);

            kaiju.motor.StopKaiju(.2f);
            
            GameObject t_doomsday = GameObject.Instantiate(doomsdayObject, start.transform.position, Quaternion.identity);
            DoomsdyRayUpgradeObject t_obj = t_doomsday.GetComponent<DoomsdyRayUpgradeObject>();

            t_obj.Init(this, t_pos);
        }
    }
}
