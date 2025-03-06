using Mekaiju.Attribute;
using Mekaiju.Entity.Effect;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Passive
{
    public class BallDefense : IPassive
    {
        [Separator]
        public float duration = 5;
        [OverrideLabel("Speed (% of kaiju speed)")]
        public float speed = 50;
        public int nbHit = 3;
        private int currentHit = 0;
        [SOSelector]
        public Effect defenseEffect;

        public override void Passive(KaijuInstance p_kaiju)
        {
            base.Passive(p_kaiju);
            if(!_using) return;
            _using = false;
            currentHit = 0;
            p_kaiju.StartCoroutine(defense(p_kaiju));
        }

        public IEnumerator defense(KaijuInstance p_kaiju)
        {
            var t_effet = p_kaiju.AddEffect(defenseEffect);
            float t_time = 0;
            while(t_time < duration)
            {
                Vector3 t_posBehind = p_kaiju.motor.GetPositionBehind(15);
                p_kaiju.motor.BackOff(t_posBehind, speed);
                yield return new WaitForSeconds(0.01f);
                t_time += 0.01f;
            }
            isUsed = false;
            p_kaiju.RemoveEffect(t_effet);
        }

        public override void OnDamage()
        {
            if (!_canUse) return;
            base.OnDamage();
            currentHit++;
            if (currentHit >= nbHit)
            {
                Active();
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            currentHit = 0;
        }

        public override void Active()
        {
            base.Active();
            currentHit = 0;
        }
    }
}
