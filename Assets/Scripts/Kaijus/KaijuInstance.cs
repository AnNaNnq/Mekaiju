using Mekaiju.Attributes;
using UnityEngine;
using System.Collections.Generic;
using MyBox;
using System.Linq;
using Mekaiju.Attribute;
using Mekaiju.Utils;
using System;
using System.Collections;

namespace Mekaiju.AI
{
    [RequireComponent(typeof(KaijuBrain))]
    [RequireComponent(typeof(KaijuMotor))]
    [RequireComponent(typeof(KaijuAnimatorController))]
    public class KaijuInstance : IEntityInstance
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
        public List<StatefullEffect> effects { get; private set; }
        public InstanceContext context { get; private set; }

        public List<KaijuPassive> passives;

        [SerializeField]
        private bool _canBehaviorSwitch = true;

        public GameObject target { get; private set; }

        [HideInInspector]
        public KaijuMotor motor { get { return _motor; } }

        protected KaijuMotor _motor;

        public KaijuBrain brain { get { return _brain; } }

        private KaijuBrain _brain;

        public KaijuAnimatorController animator { get { return _animation; } }

        private KaijuAnimatorController _animation;

        [SerializeField]
        private int _currentPhase = 2;

        [SOSelector]
        [OverrideLabel("Attack Graph (Phase 1)")]
        public KaijuAttackContainer attackGraphPhaseOne;
        [SOSelector]
        [OverrideLabel("Attack Graph (Phase 2)")]
        public KaijuAttackContainer attackGraphPhaseTow;

        [Separator]
        [Header("Debug")]
        public bool checkRange;
        [ConditionalField(nameof(checkRange))] public float debugRange;
        public float dps;

        private KaijuDebug _debug;

        bool _isInFight;

        public KaijuAttackContainer GetGraph()
        {
            if (_currentPhase == 1) return attackGraphPhaseOne;
            else return attackGraphPhaseTow;
        }

        private void Start()
        {
            _motor = GetComponent<KaijuMotor>();
            _brain = GetComponent<KaijuBrain>();
            _animation = GetComponent<KaijuAnimatorController>();
            target = GameObject.FindGameObjectWithTag(targetTag);
            foreach (var behavior in behaviors)
            {
                behavior.Init(target, gameObject);
            }

            _debug = FindFirstObjectByType<KaijuDebug>();

            dps = 0;

            _currentPhase = 1;

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
            }

            foreach(var passive in passives)
            {
                passive.passive.OnStart();
            }

            CheckAllBehaviorsDisabeled();
            StartCoroutine(resetDps());
        }

        private void Update()
        {
            if (_isInFight)
            {
                _brain.StarFight();
            }
            else
            {
                UseBehavior();
            }
            effects.ForEach(effect => effect.Tick());
            effects.RemoveAll(effect =>
            {
                if (effect.state == EffectState.Expired)
                {
                    effect.Dispose();
                    return true;
                }
                return false;
            });

        }

        private void FixedUpdate()
        {
            effects.ForEach(effect => effect.FixedTick());
        }

        public void UseBehavior()
        {
            foreach(var behavior in behaviors)
            {
                behavior.Trigger();
                if (behavior.active) behavior.Run();
            }
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

        public void Combat()
        {
            _isInFight = true;
        }

        public bool TargetInRange(float p_range)
        {
            return Vector3.Distance(target.transform.position, transform.position) <= p_range;
        }


        #region setters & getters

        public Vector3 GetTargetPos()
        {
            return target.transform.position;
        }

        /// <summary>
        /// Adds a new effect to the list of active effects without a timeout. 
        /// The effect will remain active indefinitely until it is manually removed.
        /// </summary>
        /// <param name="p_effect">The effect to be added.</param>
        public IDisposable AddEffect(Effect p_effect)
        {
            effects.Add(new(this, p_effect));
            return effects[^1];
        }

        /// <summary>
        /// Adds a new effect to the list of active effects, with a specified duration.
        /// </summary>
        /// <param name="p_effect">The effect to be added.</param>
        /// <param name="p_time">The duration of the effect in seconds.</param>
        public IDisposable AddEffect(Effect p_effect, float p_time)
        {
            effects.Add(new(this, p_effect, p_time));
            return effects[^1];
        }

        /// <summary>
        /// Remove an effect.
        /// </summary>
        /// <param name="p_effect">The effect to remove.</param>
        public void RemoveEffect(IDisposable p_effect)
        {
            if (typeof(StatefullEffect).IsAssignableFrom(p_effect.GetType()))
            {
                effects.Remove((StatefullEffect)p_effect);
                p_effect.Dispose();
            }
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
        public override EnumArray<ModifierTarget, ModifierCollection> modifiers => context.modifiers;

        public override float baseHealth => bodyParts.Aggregate(0f, (t_acc, t_part) => t_acc + t_part.health);

        public override bool isAlive => !bodyParts.All(t_part => t_part.isDestroyed);

        public void Heal(GameObject p_bodyPart, float p_amonunt)
        {
            BodyPart t_part = GetBodyPartWithGameObject(p_bodyPart);
            Heal(t_part, p_amonunt);
        }

        public void Heal(BodyPart p_bodyPart, float p_amonunt)
        {
            p_bodyPart.health += p_amonunt;
            if (p_bodyPart.isDestroyed && p_bodyPart.health > 0)
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
            p_bodyPart.health -= p_amonunt;
            if(!p_bodyPart.isDestroyed && p_bodyPart.health <= 0)
            {
                p_bodyPart.isDestroyed = true;
            }

            UpdateUI();

            foreach (var passive in passives)
            {
                passive.passive.OnDamage();
            }

            onTakeDamage.Invoke(p_amonunt);
        }

        public override void TakeDamage(float p_damage)
        {
            float t_amountForPart = p_damage / bodyParts.Count();
            foreach (var part in bodyParts)
            {
                TakeDamage(part, t_amountForPart);
            }
        }
        #endregion

        #region debug
        private void OnDrawGizmos()
        {
            foreach (var behavior in behaviors.Where(b => b.showGizmo))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, behavior.triggerArea);
            }

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
