using Mekaiju.Attribute;
using Mekaiju.Entity;
using Mekaiju.Entity.Effect;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class RockLook : Attack
    {
        [Separator]
        [OverrideLabel("Look Duration (sec)")]
        public float lookDuration = 2f;
        [SOSelector]
        public Effect successEffect;
        [OverrideLabel("Effect Duration (sec)")]
        public float effectDuration = 2f;

        MeshRenderer _rend;

        Transform _start;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            _start = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;
            _kaiju.motor.StopKaiju();

            _rend = _start.GetComponent<MeshRenderer>();
            p_kaiju.StartCoroutine(AttackEnumerator());
        }

        public override IEnumerator AttackEnumerator()
        {
            base.AttackEnumerator();

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
            _kaiju.motor.StartKaiju();
            Debug.Log(_rend.isVisible);
            if(_rend.isVisible)
            {
                MechaInstance t_player = _kaiju.target.GetComponent<MechaInstance>();
                t_player.AddEffect(successEffect, effectDuration);
            }
            OnEnd();
        }
    }
}