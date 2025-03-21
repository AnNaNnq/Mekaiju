using Mekaiju.Attribute;
using Mekaiju.Entity;
using MyBox;
using UnityEngine;


namespace Mekaiju.AI.Attack
{
    public class AbyssalVortex : Attack
    {
        [Separator]
        [OverrideLabel("Gravitational zone prefab")][OpenPrefabButton] public GameObject gameObjectAbyssalVortex;
        [OverrideLabel("Kaillou prefab")][OpenPrefabButton] public GameObject gameObjectRock;
        [OverrideLabel("Vortex Radius")] public float radius = 10f;
        [OverrideLabel("Number of rock")] public int nbRock = 10;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            _kaiju.motor.StopKaiju(2f);
            GameObject t_zone = GameObject.Instantiate(gameObjectAbyssalVortex, _kaiju.GetTargetPos(), Quaternion.identity);
            GravitationalZone t_gz = t_zone.GetComponent<GravitationalZone>();           
            t_gz.SetUp(_kaiju, this);
            OnEnd();
        }
    }
}
