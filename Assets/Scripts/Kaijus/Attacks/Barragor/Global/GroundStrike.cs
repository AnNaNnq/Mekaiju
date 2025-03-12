using Mekaiju.AI.Attack.Instance;
using Mekaiju.Entity;
using Mekaiju.Utils;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class GroundStrike : Attack
    {
        [Separator]
        public float timeBeforeFirstAttack = .2f;
        public float timeBeforeSecondAttack = .2f;
        public GameObject shockwavePrefab;
        public float shockwaveSpeed = 10f;
        public float maxRadius = 30f;
        public float repulsionForce = 1f;
        public float verticalForce = 10f;

        Transform _start;

        KaijuInstance _kaiju;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)p_kaiju;
            t_kaiju.animator.AttackAnimation(nameof(GroundStrike));
            t_kaiju.motor.StopKaiju();
            _kaiju = t_kaiju;

            _start = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;
        }

        public override void onAction()
        {
            base.onAction();

            Vector3 t_pos = _start.position;
            t_pos = new Vector3(t_pos.x, UtilsFunctions.GetGround(t_pos), t_pos.z);
            GameObject t_go = GameObject.Instantiate(shockwavePrefab, t_pos, Quaternion.identity);
            ShockWave t_sw = t_go.GetComponent<ShockWave>();
            t_sw.SetUp(this);
        }

        public override void onEnd()
        {
            base.onEnd();
            _kaiju.motor.StartKaiju();
            _kaiju.StartCoroutine(UtilsFunctions.CooldownRoutine(cooldown, () => canUse = true));
        }
    }
}
