using Mekaiju.Attribute;
using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


namespace Mekaiju.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class TutoAI : BasicAI
    {
        #region Attaque de face
        [Foldout("Attaque de face")]
        [OverrideLabel("Damage")]
        public int attackDmg = 5;
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
        [OverrideLabel("Attack zone center")]
        public Vector3 attackZoneCenter;
        [OverrideLabel("Attack zone size")]
        public Vector3 attackZoneSize;
        [OverrideLabel("Use from phase")]
        public int attackPhaseStart = 0;
        [OverrideLabel("Stop use form phase (0 = never stop)")]
        public int attackPhaseStop = 0;

        private bool _canBaseAttack = true;
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
        [SelectFromList(nameof(bodyParts))] public int chargeBody;
        [OverrideLabel("Charge Speed")]
        public float chargeSpeed = 10;
        public float chargeDuration = 0.5f;
        [OverrideLabel("Countdown (sec)")]
        public float chargeCoutdown = 2;
        [OverrideLabel("Stop Distance")]
        public float stopChargeDistance = 10;
        [OverrideLabel("Time Prep Before Charge (sec)")]
        public float chargePrepTime = 1;
        [OverrideLabel("Attak zone center")]
        public Vector3 chargeZoneCenter;
        [OverrideLabel("Attak zone size")]
        public Vector3 chargeZoneSize;
        [OverrideLabel("Use from phase")]
        public int chargePhaseStart = 0;
        [OverrideLabel("Stop use form phase (0 = never stop)")]
        public int chargePhaseStop = 0;

        private bool _isCharging = false;
        private bool _canCharge = true;
        #endregion

        #region Defense
        [Foldout("D�fense Tite Boule")]
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
        [OverrideLabel("Countdown (sec)")]
        public float defenseCoutdown = 1;
        [OverrideLabel("Use from phase")]
        public int defensePhaseStart = 0;
        [OverrideLabel("Stop use form phase (0 = never stop)")]
        public int defensePhaseStop = 0;

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
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int bigBody;
        [SOSelector]
        [OverrideLabel("Effect")]
        public Effect effect;
        [OverrideLabel("Effect duration (sec)")]
        public float bigEffectDuration = 2;
        [OverrideLabel("Countdown (sec)")]
        public float bigCoutdown = 1;
        [OverrideLabel("Attack zone center")]
        public Vector3 bigZoneCenter;
        [OverrideLabel("Attack zone size")]
        public Vector3 bigZoneSize;
        [OverrideLabel("Use from phase")]
        public int bigPhaseStart = 1;
        [OverrideLabel("Stop use form phase (0 = never stop)")]
        public int bigPhaseStop = 0;

        private bool _isBigAttack = false;
        private bool _canBigAttack = true;
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
        [OverrideLabel("Show Gizmo For Crocs")]
        public bool debugCrocs = false;
        [ConditionalField(nameof(debugCrocs))] public Color colorForCrocsMaxRange = Color.green;
        #endregion

        /// <summary>
        /// Function called when an enemy is in aggressive mode
        /// </summary>
        public override void Agro()
        {
            base.Agro();
            AttackStateMachine();
        }


        /// <summary>
        /// Attack status machine
        /// </summary>
        public void AttackStateMachine()
        {
            if (_isCharging) return;
            if(_isDefense) return;
            float t_dist = Vector3.Distance(transform.position, _target.transform.position);
            //Defense
            if(_currentHitsDefense >= hitsNumberDefense && _canDefense && !_isDefense && _canAttack && _currentPhase >= defensePhaseStart && (defensePhaseStop == 0 || _currentPhase < defensePhaseStop))
            {
                _canAttack = false;
                _isDefense = true;
                _canDefense = false;
                _agent.speed = defenseSpeed;
                StartCoroutine(Defense());
            }
            //Big fang
            else if (t_dist < bigAttackRange && _canBigAttack && !_isBigAttack && _canAttack && _currentPhase >= bigPhaseStart && (bigPhaseStop == 0 || _currentPhase < bigPhaseStop))
            {
                _canAttack = false;
                _agent.enabled = true;
                _agent.isStopped = false;
                _agent.speed = normalSpeed;
                StartCoroutine(BigAttack());
            }
            //Face attack
            else if (t_dist < chargeRangeMin && _canAttack && t_dist > attackRange && _currentPhase >= attackPhaseStart && (attackPhaseStop == 0 || _currentPhase < attackPhaseStop))
            {
                _agent.enabled = true;
                _agent.isStopped = false;
                _agent.speed = attackRunSpeed;
                MoveTo(_target.transform.position, attackRange);
                if(_agent.remainingDistance <= attackRange)
                {
                    _canAttack = false;
                    _agent.isStopped = true;
                    _agent.enabled = false;
                    if (_attackActive) return;
                    StartCoroutine(FaceAttack());
                }
            }
            //Charge
            else if(t_dist > chargeRangeMin && t_dist < chargeRangeMax && _canCharge && _canAttack && !_isCharging && _currentPhase >= chargePhaseStart && (chargePhaseStop == 0 || _currentPhase < chargePhaseStop))
            {
                _canAttack = false;
                StartCharge();
                _currentHitsDefense++;
            }
            // Normal status
            else
            {
                _agent.enabled = true;
                _agent.isStopped = false;
                _agent.speed = normalSpeed;
                MoveTo(_target.transform.position, attackRange);
            }
        }

        /// <summary>_canAttack
        /// Ball defense
        /// </summary>
        /// <returns></returns>
        public IEnumerator Defense()
        {
            float t_time = 0;
            float t_duration = durationDefense;
            if (bodyParts[defenseBody].isDestroyed)
            {
                t_duration /= 2;
            }
            while (t_time < t_duration)
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
            AttackCooldown();
        }

        /// <summary>
        /// Front attack
        /// </summary>
        /// <returns></returns>
        public IEnumerator FaceAttack()
        {
            _attackActive = true;
            for (int i = 0; i < attackCount; i++)
            {
                int t_dmg = attackDmg;
                if (bodyParts[attackBody].isDestroyed)
                {
                    t_dmg /= 2;
                }
                Attack(t_dmg, attackZoneCenter, attackZoneSize);
                yield return new WaitForSeconds(attackCoutdownBetween);
            }
            _canBaseAttack = false;
            yield return new WaitForSeconds(attackCoutdown);
            _canBaseAttack = true;
            _attackActive = false;

            _agent.enabled = true;
            _agent.isStopped = false;
            AttackCooldown();
        }

        /// <summary>
        /// Big fang attack
        /// </summary>
        /// <returns></returns>
        public IEnumerator BigAttack()
        {
            _canBigAttack = false;
            float t_dist = Vector3.Distance(transform.position, _target.transform.position);

            while (t_dist < bigAttackRange)
            {
                _isBigAttack = true;
                LookTarget();
                int t_dmg = bigAttackDmg;
                if (bodyParts[bigBody].isDestroyed)
                {
                    t_dmg /= 2;
                }
                Attack(t_dmg, bigZoneCenter, bigZoneSize, effect, bigEffectDuration);
                _isBigAttack = false;
                float i = 0;
                while (i < bigCoutdown)
                {
                    LookTarget();
                    yield return new WaitForSeconds(0.01f);
                    i += 0.01f;
                }
                t_dist = Vector3.Distance(transform.position, _target.transform.position);
            }
            _canBigAttack = true;
            AttackCooldown();
        }

        /// <summary>
        /// Start charging
        /// </summary>
        public void StartCharge()
        {
            _isCharging = true;

            _agent.isStopped = true;
            _agent.enabled = false;

            
            StartCoroutine(ChargeCoroutine());
        }

        /// <summary>
        /// Launches charge (displacement + damage)
        /// </summary>
        /// <returns></returns>
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
            int t_dmg = chargeDmg;
            if (bodyParts[attackBody].isDestroyed)
            {
                t_dmg /= 2;
            }
            Attack(t_dmg, chargeZoneCenter, chargeZoneSize);
            _agent.enabled = true;
            _agent.isStopped = false;
            _isCharging = false;
            _canCharge = false;
            AttackCooldown();
            StartCoroutine(CooldownRoutine(chargeCoutdown, () => _canCharge = true));
        }

        /// <summary>
        /// Rewrite TakeDamage function to take defense into account
        /// </summary>
        /// <param name="p_damage"></param>
        /// <param name="p_tuchObject"></param>
        public override void TakeDamage(int p_damage, GameObject p_tuchObject)
        {
            int t_damage = p_damage;
            if (_isDefense) t_damage -= (int)(damagePercentageDefense * t_damage) / 100;
            base.TakeDamage(t_damage, p_tuchObject);
        }



        #region Fonctions pour les LD

        protected new void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (debugAttak1)
            {
                Gizmos.color = colorForFaceAttackRange;
                Gizmos.DrawWireSphere(transform.position, attackRange);
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(transform.position + transform.rotation * attackZoneCenter, attackZoneSize);
            }
            if (debugAttak2) {
                Gizmos.color = colorForChargeMaxRange;
                Gizmos.DrawWireSphere(transform.position, chargeRangeMax);
                Gizmos.color = colorForChargeMinRange;
                Gizmos.DrawWireSphere(transform.position, chargeRangeMin);
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(transform.position + transform.rotation * chargeZoneCenter, chargeZoneSize);
            }
            if (debugCrocs)
            {
                Gizmos.color = colorForCrocsMaxRange;
                Gizmos.DrawWireSphere(transform.position, bigAttackRange);
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(transform.position + transform.rotation * bigZoneCenter, bigZoneSize);
            }
        }
        #endregion
    }
}
