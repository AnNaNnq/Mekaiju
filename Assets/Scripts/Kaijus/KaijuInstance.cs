using Mekaiju.Attributes;
using UnityEngine;
using System.Collections.Generic;
using MyBox;
using System.Linq;
using Mekaiju.Attribute;
using System;
using System.Collections;
using Mekaiju.Entity;
using Mekaiju.AI.Objet;
using Mekaiju.AI.Body;
using Mekaiju.AI.Behavior;

namespace Mekaiju.AI
{
    [RequireComponent(typeof(KaijuBrain))]
    [RequireComponent(typeof(KaijuMotor))]
    [RequireComponent(typeof(KaijuAnimatorController))]
    [RequireComponent(typeof(Rigidbody))]
    public class KaijuInstance : EntityInstance
    {
        [Header("General")]
        [Tag] public string targetTag;
        public KaijuStats stats;
        public BodyPart[] bodyParts;

        [Separator]
        [field: SerializeReference, SubclassPicker]
        public List<KaijuBehavior> behaviors = new List<KaijuBehavior>();
        public float timeBetweenTowAction = 1f;

        [field: SerializeField]
        public InstanceContext context { get; private set; }

        public List<KaijuPassive> passives;

        public GameObject target { get; private set; }

        [HideInInspector]
        public KaijuMotor motor { get { return _motor; } }

        protected KaijuMotor _motor;

        public Rigidbody rb {  get { return _rb; } }

        private Rigidbody _rb;

        public KaijuBrain brain { get { return _brain; } }

        private KaijuBrain _brain;

        public KaijuAnimatorController animator { get { return _animation; } }

        private KaijuAnimatorController _animation;

        public int currentPhase = 2;

        [SOSelector]
        [OverrideLabel("Attack Graph (Phase 1)")]
        public KaijuAttackContainer attackGraphPhaseOne;
        [SOSelector]
        [OverrideLabel("Attack Graph (Phase 2)")]
        public KaijuAttackContainer attackGraphPhaseTow;

        [Separator]
        public KaijuPhaseAttack changePhaseAction;

        [Separator]
        [Header("Debug")]
        public bool checkRange;
        [ConditionalField(nameof(checkRange))] public float debugRange;
        public float dps;

        private KaijuDebug _debug;

        bool _isInFight;

        public event Action<Collision> OnCollision;

        [Header("Pas touche")]
        public KaijuCollsionDetector detector;

        public KaijuAttackContainer GetGraph()
        {
            if (currentPhase == 1) return attackGraphPhaseOne;
            else return attackGraphPhaseTow;
        }

        private void Start()
        {
            _motor = GetComponent<KaijuMotor>();
            _brain = GetComponent<KaijuBrain>();
            _animation = GetComponent<KaijuAnimatorController>();
            _rb = GetComponent<Rigidbody>();
            target = GameObject.FindGameObjectWithTag(targetTag);
            foreach (var behavior in behaviors)
            {
                behavior.Init(target, gameObject);
            }

            _debug = FindFirstObjectByType<KaijuDebug>();

            dps = 0;

            currentPhase = 1;

            // We add the BodyPartObject script to bodyParts objects if they don't already have it
            foreach (BodyPart t_part in bodyParts)
            {
                foreach (GameObject t_obj in t_part.part)
                {
                    if (t_obj.GetComponent<BodyPartObject>() == null)
                    {
                        t_obj.AddComponent<BodyPartObject>();
                    }
                }
                t_part.currentHealth = t_part.maxHealth;
            }

            foreach(var passive in passives)
            {
                passive.passive.OnStart();
            }

            context = new();
            if(changePhaseAction != null)
            {
                changePhaseAction.attack.Init(this);
            }

            CheckAllBehaviorsDisabeled();
            StartCoroutine(resetDps());
        }

        public override void Update()
        {
            base.Update();
            if (_isInFight)
            {
                _brain.StarFight();
            }
            else
            {
                UseBehavior();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public void UseBehavior()
        {
            foreach(var behavior in behaviors)
            {
                behavior.Trigger();
                if (behavior.active) behavior.Run();
            }
        }

        public void ChangePhase()
        {
            if(changePhaseAction != null)
            {
                motor.StopKaiju();
                changePhaseAction.attack.Action();
            }
        }

        public void SetPhase(int p_phase)
        {
            currentPhase = p_phase;
            _brain.ResetAttack();
        }

        public bool canSwitch()
        {
            return behaviors.All(b => !b.active || !b.triggerOnce);
        }

        public void CheckAllBehaviorsDisabeled()
        {
            if (behaviors.Count(b => !b.active) == behaviors.Count)
            {
                foreach (var behavior in behaviors.Where(b => !b.canTrigger))
                {
                    behavior.Active();
                }
            }
        }

        public float GetRealDamage(float p_amonunt)
        {
            var t_damage = modifiers[Statistics.Damage].ComputeValue(p_amonunt);
            return stats.dmg * (t_damage/100);
        }

        public float GetRealSpeed(float p_amonunt)
        {
            var t_amount = modifiers[Statistics.Speed].ComputeValue(p_amonunt);
            return stats.speed * (t_amount / 100);

        }

        public void Combat()
        {
            motor.SetSpeed(100);
            _isInFight = true;
        }

        public float TargetInRange()
        {
            return Vector3.Distance(target.transform.position, transform.position);
        }

        public bool TargetInRange(float p_range)
        {
            return Vector3.Distance(target.transform.position, transform.position) <= p_range;
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnCollision?.Invoke(collision);  // Passe la collision � l'�v�nement
        }

        #region setters & getters

        public Vector3 GetTargetPos()
        {
            return target.transform.position;
        }

        /// <summary>
        /// Get the body part with the GameObject
        /// </summary>
        /// <param name="p_object"></param>
        /// <returns></returns>
        public BodyPart GetBodyPartWithGameObject(GameObject p_object)
        {
            foreach (BodyPart t_part in bodyParts)
            {
                foreach (GameObject t_obj in t_part.part)
                {
                    if (t_obj == p_object)
                    {
                        return t_part;
                    }
                }
            }
            return null;
        }

        public List<KaijuPassive> GetPassivesActive()
        {
            List<KaijuPassive> t_passives = new List<KaijuPassive>();
            foreach (var passive in passives)
            {
                if (passive.passive.isUsed)
                {
                    t_passives.Add(passive);
                }
            }
            return t_passives;
        }

        #endregion

        #region implemation of IEntityInstance
        public override float baseHealth => bodyParts.Sum(p => p.maxHealth);

        public override float health => bodyParts.Sum(p => p.currentHealth);

        public override bool isAlive => !bodyParts.All(t_part => t_part.isDestroyed);

        public void Heal(GameObject p_bodyPart, float p_amonunt)
        {
            BodyPart t_part = GetBodyPartWithGameObject(p_bodyPart);
            Heal(t_part, p_amonunt);
        }

        public void Heal(BodyPart p_bodyPart, float p_amonunt)
        {
            p_bodyPart.currentHealth += p_amonunt;
            if (p_bodyPart.isDestroyed && p_bodyPart.currentHealth > 0)
            {
                p_bodyPart.isDestroyed = false;
            }
        }

        public override void Heal(float p_amount)
        {
            float t_amountForPart = p_amount / bodyParts.Count();
            foreach(var part in bodyParts)
            {
                Heal(part, t_amountForPart);
            }
        }

        public void TakeDamage(GameObject p_bodyPart, float p_amonunt)
        {
            BodyPart t_part = GetBodyPartWithGameObject(p_bodyPart);
            TakeDamage(t_part, p_amonunt);
        }

        public void TakeDamage(BodyPart p_bodyPart, float p_amonunt)
        {
            var t_defense = modifiers[Statistics.Defense].ComputeValue(stats.def);

            var t_realDamage = p_amonunt * (1- (t_defense/100));

            p_bodyPart.currentHealth -= p_amonunt;
            p_bodyPart.currentHealth = MathF.Max(0, p_bodyPart.currentHealth);

            if (!p_bodyPart.isDestroyed && p_bodyPart.currentHealth <= 0)
            {
                p_bodyPart.isDestroyed = true;
            }

            UpdateUI();

            foreach (var passive in passives)
            {
                passive.passive.OnDamage();
            }

            onTakeDamage.Invoke(p_amonunt);

            if (IsDeath())
            {
                Destroy(gameObject);
            }

            if(currentPhase == 1 && (health <= (baseHealth / 50)))
            {
                ChangePhase();
            }
        }

        public override void TakeDamage(float p_damage)
        {
            float t_amountForPart = p_damage / bodyParts.Count();
            foreach (var part in bodyParts)
            {
                TakeDamage(part, t_amountForPart);
            }
        }

        public bool IsDeath()
        {
            foreach(BodyPart t_part in bodyParts)
            {
                if (!t_part.isDestroyed)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region debug
        private void OnDrawGizmos()
        {
            if (checkRange)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, debugRange);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + transform.forward * 7f, 4f);
        }

        public IEnumerator resetDps()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                dps = 0;
                UpdateUI();
            }
        }

        public void AddDPS(float p_amount)
        {
            dps += p_amount;
        }

        public void UpdateUI()
        {
            if(_debug != null) _debug.UpdateUI();
        }
        #endregion
    }
}
