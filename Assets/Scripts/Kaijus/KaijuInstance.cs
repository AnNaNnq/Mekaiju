using Mekaiju.Attributes;
using UnityEngine;
using System.Collections.Generic;
using MyBox;
using System.Linq;
using Mekaiju.Attribute;
using Mekaiju.Utils;
using System;

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

        [Separator]
        [field: SerializeReference, SubclassPicker]
        public List<KaijuBehavior> behaviors = new List<KaijuBehavior>();
        public float timeBetweenTowAction = 1f;
        
        [field: SerializeField]
        public List<StatefullEffect> effects { get; private set; }

        [SerializeField]
        private bool _canBehaviorSwitch = true;

        public GameObject target { get; private set; }

        [HideInInspector]
        public KaijuMotor motor { get { return _motor; } }

        protected KaijuMotor _motor;

        public KaijuBrain brain { get { return _brain; } }

        private KaijuBrain _brain;

        [SOSelector]
        public KaijuAttackContainer attackGraph;

        [Separator]
        [Header("Debug")]
        public bool checkRange;
        [ConditionalField(nameof(checkRange))] public float debugRange ;

        private void Start()
        {
            _motor = GetComponent<KaijuMotor>();
            _brain = GetComponent<KaijuBrain>();
            target = GameObject.FindGameObjectWithTag(targetTag);
            foreach (var behavior in behaviors)
            {
                behavior.Init(target, gameObject);
            }

            CheckAllBehaviorsDisabeled();
        }

        private void Update()
        {
            UseBehavior();
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
                behavior.IsTrigger();
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
            _brain.StarFight();
        }

        public bool TargetInRange(float p_range)
        {
            return Vector3.Distance(target.transform.position, transform.position) <= p_range;
        }

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

        #region IEntityInstance implemetation

        // TODO: implement
        public override EnumArray<ModifierTarget, ModifierCollection> modifiers => throw new System.NotImplementedException();

        public override float baseHealth => throw new System.NotImplementedException();

        public override void Heal(float p_amount)
        {
            throw new System.NotImplementedException();
        }

        public override void TakeDamage(float p_damage)
        {
            throw new System.NotImplementedException();
        }
#endregion
    }
}
