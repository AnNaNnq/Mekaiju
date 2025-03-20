using Mekaiju.Attribute;
using Mekaiju.Entity.Effect;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI.Attacl
{
    [System.Serializable]
    public class ShockWaveStat
    {
        public GameObject shockwavePrefab;
        public float shockwaveSpeed = 10f;
        public float shockwaveDamage = 50f;
        public float maxRadius = 30f;
        [SOSelector]
        public Effect effect;
        public float effectDuration = 0.2f;

        [HideInInspector] public KaijuInstance kaiju;
    }

}

