using Mekaiju.Entity;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class ShatteringJump : ChockWave
    {
        [Separator("Jump")]
        public float jumpForce = 100f;

        [Separator("Electric Zone")]
        public GameObject electricZonePrefab;
        public DamageZoneStats electricZone;
        public float zoneDuration = 10f;
        int stat = 0;

        public override void Init()
        {
            base.Init();

            stat = 0;
        }

        public override void OnGround()
        {
            base.OnGround();
            if(stat == 1)
            {
                _kaiju.animator.AttackAnimation("Grounded");
                LunchWave();
                stat = 2;
            }
        }

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            _kaiju.animator.AttackAnimation("Jump");
        }

        public override void OnAction()
        {
            base.OnAction();
            if(stat == 0)
            {
                _kaiju.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                stat = 1;
            }
            else if (stat == 2)
            {
                GameObject t_obj = GameObject.Instantiate(electricZonePrefab, _kaiju.transform.position, Quaternion.identity);
                DamageZone t_dmg = t_obj.GetComponent<DamageZone>();
                t_dmg.Init(electricZone, _kaiju);
                GameObject.Destroy(t_obj, zoneDuration);
            }
        }

        
    }
}
