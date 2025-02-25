using Mekaiju.AI.Attack;
using Mekaiju.Attribute;
using Mekaiju.Utils;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI
{
    public class DoomsdayRay : IAttack
    {
        [Separator]
        public int damage = 10;
        [OverrideLabel("Prefab")][OpenPrefabButton] public GameObject doomsdayObject;
        [HideInInspector] public Transform start;
        [OverrideLabel("Ray Speed")] public float speed = 10f;
        [OverrideLabel("Duration (sec)")] public float duration = 5f;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            start = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;
            _using = true;
            kaiju.motor.StopKaiju(duration);
            Vector3 t_pos = new Vector3(kaiju.transform.position.x, UtilsFunctions.GetGround(kaiju.transform.position), kaiju.transform.position.z) + (kaiju.transform.forward * 10);
            GameObject t_doomsday = GameObject.Instantiate(doomsdayObject, t_pos, Quaternion.identity);
            DoomsdayRaySpawnable t_dr = t_doomsday.GetComponent<DoomsdayRaySpawnable>();
            t_dr.SetUp(start, this, kaiju);
            GameObject.Destroy(t_doomsday, duration);
        }
    }
}
