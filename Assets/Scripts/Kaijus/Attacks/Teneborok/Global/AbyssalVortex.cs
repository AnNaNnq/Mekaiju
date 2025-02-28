using Mekaiju.Attribute;
using MyBox;
using UnityEngine;


namespace Mekaiju.AI.Attack
{
    public class AbyssalVortex : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        [OverrideLabel("Gravitational zone prefab")][OpenPrefabButton] public GameObject gameObjectAbyssalVortex;
        [OverrideLabel("Kaillou prefab")][OpenPrefabButton] public GameObject gameObjectRock;
        [OverrideLabel("Vortex Radius")] public float radius = 10f;
        [OverrideLabel("Number of rock")] public int nbRock = 10;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            kaiju.motor.StopKaiju(2f);
            GameObject t_zone = GameObject.Instantiate(gameObjectAbyssalVortex, kaiju.GetTargetPos(), Quaternion.identity);
            GravitationalZone t_gz = t_zone.GetComponent<GravitationalZone>();           
            t_gz.SetUp(kaiju, this);
        }
    }
}
