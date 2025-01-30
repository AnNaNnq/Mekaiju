using Mekaiju.Attribute;
using MyBox;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mekaiju.AI
{
    public class TestAI : BasicAI
    {
        [Foldout("Attaque de face")]
        [OverrideLabel("Damage")]
        public int attackmg = 5;
        [PositiveValueOnly]
        [OverrideLabel("Range")]
        public float attackRange = 5;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int attackBody;
        [OverrideLabel("Attack count")]
        public int attackCount = 2;
        [OverrideLabel("Countdown (sec)")]
        public float attackCoutdown = 2;
        [OverrideLabel("Time between two attack (sec)")]
        public float attackCoutdownBetween = 2;
        [OverrideLabel("Run Speed")]
        public float attackRunSpeed = 8;
        [SerializeField]
        private bool _canAttack = true;
        private bool _attackActive = false;

        [Foldout("Charge")]
        [OverrideLabel("Damage")]
        public int chargeDmg = 5;
        [PositiveValueOnly]
        [OverrideLabel("Range max")]
        public float chargeRangeMax = 25;
        [OverrideLabel("Range min")]
        public float chargeRangeMin = 20;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int attack2Body;
        [OverrideLabel("Charge Speed")]
        public float chargeSpeed = 10;
        public float chargeDuration = 0.5f;
        [OverrideLabel("Countdown (sec)")]
        public float chargeCoutdown = 2;
        public float stopChargeDistance = 10;

        private bool _isCharging = false;
        private bool _canCharge = true;

        [Foldout("Debug")]
        [OverrideLabel("Show Gizmo For Face Attack")]
        public bool debugAttak1 = false;
        [ConditionalField(nameof(debugAttak1))] public Color colorForFaceAttackRange;
        [OverrideLabel("Show Gizmo For Range Attack")]
        public bool debugAttak2 = false;
        [ConditionalField(nameof(debugAttak2))] public Color colorForChargeMaxRange = Color.blue;
        [ConditionalField(nameof(debugAttak2))] public Color colorForChargeMinRange = Color.yellow;
        public TextMeshProUGUI textDPS;

        private int dps = 0;

        //public int vie = 100;

        public override void Agro()
        {
            base.Agro();
            AttackStateMachine();
        }

        private new void Update()
        {
            base.Update();
            //if (Input.GetKeyDown(KeyCode.K))
            //{
            //    vie -= 10;
            //}
        }

        private new void Start()
        {
            base.Start();
            StartCoroutine(ShowDPS());
            textDPS.text = dps.ToString();
        }


        public void AttackStateMachine()
        {
            float t_dist = Vector3.Distance(transform.position, _target.transform.position);
            //Run for face attack
            if (t_dist < chargeRangeMin && t_dist > attackRange)
            {
                _agent.speed = attackRunSpeed;
                MoveTo(_target.transform.position, attackRange);
            }
            //face attack
            else if(t_dist <= attackRange && _canAttack && !_attackActive)
            {
                StartCoroutine(FaceAttack());
                _agent.speed = normalSpeed;
            }
            //Charge
            else if(t_dist > chargeRangeMin && t_dist < chargeRangeMax && _canCharge && !_isCharging)
            {
                StartCharge();
            }
            //Etat Normal
            else
            {
                _agent.speed = normalSpeed;
                MoveTo(_target.transform.position, attackRange);
            }
        }
        public IEnumerator FaceAttack()
        {
            _attackActive = true;
            while (Vector3.Distance(transform.position, _target.transform.position) <= attackRange)
            {
                if (_canAttack)
                {
                    for (int i = 0; i < attackCount; i++)
                    {
                        dps += attackmg;
                        textDPS.text = dps.ToString();
                        Debug.Log("Attack");
                        yield return new WaitForSeconds(attackCoutdownBetween);
                    }
                    _canAttack = false;
                    yield return new WaitForSeconds(attackCoutdown);
                    _canAttack = true;
                }
                else
                {
                    yield return null;
                }
            }
            _attackActive = false;
        }

        public void StartCharge()
        {
            _isCharging = true;

            _agent.isStopped = true;
            _agent.enabled = false;

            Vector3 t_targetPos = _target.transform.position;
        }

        private IEnumerator ChargeCoroutine(Vector3 p_targetPosition)
        {
            float t_elapsedTime = 0f;
            Vector3 t_startPos = transform.position;
            Vector3 t_direction = (p_targetPosition - t_startPos).normalized;
            Vector3 t_targetPos = p_targetPosition - t_direction * stopChargeDistance;

            t_targetPos.y = t_startPos.y;

            while (t_elapsedTime < chargeDuration)
            {
                transform.position = Vector3.Lerp(t_startPos, t_targetPos, t_elapsedTime / chargeDuration);
                t_elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = t_targetPos;

            _agent.enabled = true;
            _agent.isStopped = false;
            _isCharging = false;
            _canCharge = false;
            StartCoroutine(ChargeCountdown());
        }

        IEnumerator ChargeCountdown()
        {
            yield return new WaitForSeconds(chargeCoutdown);
            _canCharge = true;
        }














        protected new void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (debugAttak1)
            {
                Gizmos.color = colorForFaceAttackRange;
                Gizmos.DrawWireSphere(transform.position, attackRange);
            }
            if (debugAttak2) {
                Gizmos.color = colorForChargeMaxRange;
                Gizmos.DrawWireSphere(transform.position, chargeRangeMax);
                Gizmos.color = colorForChargeMinRange;
                Gizmos.DrawWireSphere(transform.position, chargeRangeMin);
            }
        }

        IEnumerator ShowDPS()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                dps = 0;
                textDPS.text = dps.ToString();
            }
        }
    }
}
