using Mekaiju.Attribute;
using Mekaiju.Entity.Effect;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class RockLook : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        [OverrideLabel("Look Duration (sec)")]
        public float lookDuration = 2f;
        [SOSelector]
        public Effect successEffect;
        [OverrideLabel("Effect Duration (sec)")]
        public float effectDuration = 2f;

        MeshRenderer _rend;

        Transform _start;

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            _start = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;
            kaiju.motor.StopKaiju();

            _rend = _start.GetComponent<MeshRenderer>();
            kaiju.StartCoroutine(Attack(kaiju));
        }

        public override IEnumerator Attack(KaijuInstance kaiju)
        {
            base.Attack(kaiju);
            bool t_stop = false;
            float t_time = 0;
            while (!t_stop)
            {
                yield return new WaitForSeconds(.1f);
                t_stop = !_rend.isVisible;
                t_time += .1f;
                if(t_time >= lookDuration)
                {
                    t_stop = true;
                }
            }
            kaiju.motor.StartKaiju();
            Debug.Log(_rend.isVisible);
            if(_rend.isVisible)
            {
                MechaInstance t_player = kaiju.target.GetComponent<MechaInstance>();
                t_player.AddEffect(successEffect, effectDuration);
            }
        }
    }
}