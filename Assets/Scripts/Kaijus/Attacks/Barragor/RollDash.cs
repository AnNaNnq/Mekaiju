using Mekaiju.AI.Attack.Instance;
using Mekaiju.Attribute;
using Mekaiju.Entity;
using Mekaiju.Entity.Effect;
using MyBox;
using System.Collections;
using UnityEngine;
namespace Mekaiju.AI.Attack
{
    public class RollDash : Attack
    {
        [Separator]
        public float attackRange = 10f;
        public float rollSpeed = 200f;
        public GameObject growingZonePrefab;

        [SOSelector]
        public Effect effect;

        public float zoneDamage;
        public float damageTick;
        public float zoneDuration = 5;

        GrowingZone _gz;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            if (_kaiju.TargetInRange(range) && !_kaiju.TargetInRange(attackRange))
            {
                _kaiju.animator.AttackAnimation("Roll");
                _kaiju.motor.StartKaiju();
                _kaiju.motor.SetSpeed(rollSpeed);
                _kaiju.motor.MoveTo(_kaiju.target.transform.position, attackRange);
                _kaiju.StartCoroutine(RoolDuration());

                GameObject t_go = GameObject.Instantiate(growingZonePrefab, _kaiju.transform.position, Quaternion.identity);
                _gz = t_go.GetComponent<GrowingZone>();
                _gz.stat = this;
                _gz.kaiju = _kaiju;
                _gz.useEndPoint = _kaiju.transform;
            }
            else
            {
                Slash();
            }
        }

        IEnumerator RoolDuration()
        {
            while (Vector3.Distance(_kaiju.transform.position, _kaiju.GetTargetPos()) > attackRange)
            {
                yield return new WaitForSeconds(0.1f);
            }
            Slash();
        }
        public void Slash()
        {
            if (_gz != null)
            {
                _gz.endPoint.position = _kaiju.transform.position;
                _gz.useEndPoint = _gz.endPoint;
            }
            _kaiju.motor.SetSpeed(100);
            _kaiju.animator.AttackAnimation("RollAndDash");
            _kaiju.motor.StopKaiju();

            GameObject.Destroy(_gz, zoneDuration);
        }
        public override void OnAction()
        {
            base.OnAction();
            SendDamage(damage);
        }
    }
}
