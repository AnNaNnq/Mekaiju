using Mekaiju.Attribute;
using MyBox;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mekaiju.AI
{
    public class TestAI : BasicAI
    {
        #region Attaque de face
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
        [SerializeField]
        private bool _attackActive = false;
        #endregion

        #region Charge
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
        [OverrideLabel("Stop Distance")]
        public float stopChargeDistance = 10;
        [OverrideLabel("Time Prep Before Charge (sec)")]
        public float chargePrepTime = 1;

        private bool _isCharging = false;
        private bool _canCharge = true;
        #endregion

        #region Defense
        [Foldout("Défense Tite Boule")]
        [PositiveValueOnly]
        [OverrideLabel("Number of hits before trigger")]
        public int hitsNumberDefense = 3;
        [OverrideLabel("Duration (sec)")]
        public float durationDefense = 5;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int defenseBody;
        [OverrideLabel("Damage reduction percentage (%)")]
        public float damagePercentageDefense = 75;
        [OverrideLabel("Speed")]
        public float defenseSpeed = 2;
        [OverrideLabel("Countdown")]
        public float defenseCoutdown = 1;

        private int _currentHitsDefense = 0;
        private bool _isDefense = false;
        private bool _canDefense = true;
        #endregion

        #region Gros croc
        [Foldout("Gros Croc")]
        [OverrideLabel("Damage")]
        public int bigAttackDmg = 10;
        [OverrideLabel("Range")]
        public float bigAttackRange = 10;
        #endregion

        #region Debug
        [Foldout("Debug")]
        [OverrideLabel("Show Gizmo For Face Attack")]
        public bool debugAttak1 = false;
        [ConditionalField(nameof(debugAttak1))] public Color colorForFaceAttackRange;
        [OverrideLabel("Show Gizmo For Charge Attack")]
        public bool debugAttak2 = false;
        [ConditionalField(nameof(debugAttak2))] public Color colorForChargeMaxRange = Color.blue;
        [ConditionalField(nameof(debugAttak2))] public Color colorForChargeMinRange = Color.yellow;
        public TextMeshProUGUI textDPS;
        [OverrideLabel("Show Gizmo For Crocs")]
        public bool debugCrocs = false;
        [ConditionalField(nameof(debugCrocs))] public Color colorForCrocsMaxRange = Color.green;
        #endregion

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
        }

        private new void Start()
        {
            base.Start();
            StartCoroutine(ShowDPS());
            textDPS.text = dps.ToString();
        }


        public void AttackStateMachine()
        {
            if (_isCharging) return;
            if(_isDefense) return;
            float t_dist = Vector3.Distance(transform.position, _target.transform.position);
            //Defense
            if(_currentHitsDefense >= hitsNumberDefense && _canDefense && !_isDefense)
            {
                _isDefense = true;
                _canDefense = false;
                _agent.speed = defenseSpeed;
                StartCoroutine(Defense());
            }
            //Run for face attack
            else if (t_dist < chargeRangeMin && t_dist > attackRange)
            {
                _agent.enabled = true;
                _agent.isStopped = false;
                _agent.speed = attackRunSpeed;
                MoveTo(_target.transform.position, attackRange);
                if(_agent.remainingDistance <= attackRange)
                {
                    if(_attackActive) return;
                    _agent.isStopped = true;
                    _agent.enabled = false;
                    StartCoroutine(FaceAttack());
                }
            }
            //Charge
            else if(t_dist > chargeRangeMin && t_dist < chargeRangeMax && _canCharge && !_isCharging)
            {
                StartCharge();
                _currentHitsDefense++;
            }
            //Etat Normal
            else
            {
                _agent.enabled = true;
                _agent.isStopped = false;
                _agent.speed = normalSpeed;
                MoveTo(_target.transform.position, attackRange);
            }
        }

        public IEnumerator Defense()
        {
            float t_time = 0;
            while(t_time < durationDefense)
            {
                Vector3 t_posBehind = GetPositionBehind(10);
                BackOff(t_posBehind);
                LookTarget();
                yield return new WaitForSeconds(0.01f);
                t_time += 0.01f;
            }
            _isDefense = false;
            yield return new WaitForSeconds(defenseCoutdown);
            _canDefense = true;
            _currentHitsDefense = 0;
        }

        public IEnumerator FaceAttack()
        {
            _attackActive = true;
            for (int i = 0; i < attackCount; i++)
            {
                dps += attackmg;
                textDPS.text = dps.ToString();
                yield return new WaitForSeconds(attackCoutdownBetween);
            }
            _canAttack = false;
            yield return new WaitForSeconds(attackCoutdown);
            _canAttack = true;
            _attackActive = false;

            _agent.enabled = true;
            _agent.isStopped = false;
        }

        public void StartCharge()
        {
            _isCharging = true;

            _agent.isStopped = true;
            _agent.enabled = false;

            
            StartCoroutine(ChargeCoroutine());
        }

        private IEnumerator ChargeCoroutine()
        {
            float t_time = 0;
            Vector3 t_targetPosition = _target.transform.position;
            while (t_time < chargePrepTime)
            {
                LookTarget();
                t_targetPosition = _target.transform.position;
                yield return new WaitForSeconds(0.01f);
                t_time += 0.01f;
            }
            float t_elapsedTime = 0f;
            Vector3 t_startPos = transform.position;
            Vector3 t_direction = (t_targetPosition - t_startPos).normalized;
            Vector3 t_targetPos = t_targetPosition - t_direction * stopChargeDistance;

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



        #region Fonctions pour les LD

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
            if (debugCrocs)
            {
                Gizmos.color = colorForCrocsMaxRange;
                Gizmos.DrawWireSphere(transform.position, bigAttackRange);
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

        #endregion
    }
}
