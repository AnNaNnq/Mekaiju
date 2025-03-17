using Mekaiju.Entity;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class ShatteringJump : ChockWave
    {
        public float jumpForce = 100f;

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
        }
    }
}
