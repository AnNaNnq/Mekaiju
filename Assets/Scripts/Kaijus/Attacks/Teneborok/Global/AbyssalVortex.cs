using Mekaiju.Attribute;
using Mekaiju.Entity;
using MyBox;
using UnityEngine;


namespace Mekaiju.AI.Attack
{
    public class AbyssalVortex : IAttack
    {
        [Separator]
        [OverrideLabel("Gravitational zone prefab")][OpenPrefabButton] public GameObject gameObjectAbyssalVortex;
        [OverrideLabel("Kaillou prefab")][OpenPrefabButton] public GameObject gameObjectRock;
        [OverrideLabel("Vortex Radius")] public float radius = 10f;
        [OverrideLabel("Number of rock")] public int nbRock = 10;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(IEntityInstance kaiju)
        {
            base.Active(kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)kaiju;
            t_kaiju.motor.StopKaiju(2f);
            GameObject t_zone = GameObject.Instantiate(gameObjectAbyssalVortex, t_kaiju.GetTargetPos(), Quaternion.identity);
            GravitationalZone t_gz = t_zone.GetComponent<GravitationalZone>();           
            t_gz.SetUp(t_kaiju, this);
        }
    }
}
