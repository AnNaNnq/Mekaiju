using Mekaiju.AI.Attack.Instance;
using Mekaiju.Attribute;
using Mekaiju.Entity;
using Mekaiju.Entity.Effect;
using Mekaiju.Utils;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class GroundStrike : Attack
    {
        [Separator]
        public GameObject shockwavePrefab;
        public float shockwaveSpeed = 10f;
        public float shockwaveDamage = 50f;
        public float maxRadius = 30f;
        [SOSelector]
        public Effect effect;
        public float effectDuration = 0.2f;

        Transform _start;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            _kaiju.animator.AttackAnimation(nameof(GroundStrike));
            _kaiju.motor.StopKaiju();

            _start = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;
        }

        public override void OnAction()
        {
            base.OnAction();

            Vector3 t_pos = _start.position;
            //t_pos = new Vector3(t_pos.x, UtilsFunctions.GetGround(t_pos), t_pos.z);
            GameObject t_go = GameObject.Instantiate(shockwavePrefab, t_pos, Quaternion.identity);
            ShockWave t_sw = t_go.GetComponent<ShockWave>();
            t_sw.SetUp(this);
            SendDamage(damage);
        }
    }
}
