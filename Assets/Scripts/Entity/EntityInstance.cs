using System;
using System.Collections.Generic;
using Mekaiju.Entity.Effect;
using Mekaiju.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju.Entity
{
    /// <summary>
    /// A base class for all entity instance
    /// Allow effects on them
    /// </summary>
    public abstract class EntityInstance : MonoBehaviour, IDamageable, IStaminable, IEffectable
    {
        /// <summary>
        /// Store effect to apply to the current entity.
        /// </summary>
        private List<StatefullEffect> effects = new();

        /// <summary>
        /// Store a reference to the parent.
        /// </summary>
        public EntityInstance parent { get; protected set; } = null;

        /// <summary>
        /// Allow time point tracking
        /// Can be overridden if entity is controlled by top level one.
        /// </summary>
        public virtual EnumArray<TimePoint, float> timePoints { get; } = new(() => float.MinValue);

        /// <summary>
        /// Define usefull states on an entity
        /// </summary>
        public virtual EnumArray<State, bool> states { get; } = new(() => false);

        /// <summary>
        /// Bind base entity stats.
        /// Must be overrided to use computedStats.
        /// </summary>
        protected virtual EnumArray<Statistics, float> statistics { get; }

        /// <summary>
        /// Used to apply modifer on statistics.
        /// </summary>
        public virtual EnumArray<Statistics, ModifierCollection> modifiers { get; } = new(() => new());

        /// <summary>
        /// Compute stats with modifiers.
        /// </summary>
        /// <param name="p_kind">The targeted statistics.</param>
        /// <returns>The computed statistic.</returns>
        public virtual float ComputedStatistics(Statistics p_kind)
        {
            return modifiers[p_kind].ComputeValue(statistics[p_kind]);
        }

        public virtual UnityEvent<float> onTakeDamage { get; } = new();
        public virtual UnityEvent<float> onDealDamage { get; } = new();

        public UnityEvent<Collider> onCollide = new();
        
        public abstract bool isAlive    { get; }
        public abstract float health     { get; }
        public abstract float baseHealth { get; }

        public abstract void Heal      (float p_amount);
        public abstract void TakeDamage(float p_damage);

        public virtual float baseStamina => 0f;
        public virtual float stamina     => 0f;

        public virtual void ConsumeStamina(float p_amount) {}
        public virtual void RestoreStamina(float p_amount) {}

        public UnityEvent<StatefullEffect> onAddEffect    { get; } = new();
        public UnityEvent<StatefullEffect> onRemoveEffect { get; } = new();

        public IDisposable AddEffect(Effect.Effect p_effect)
        {
            effects.Add(new(this, p_effect));
            onAddEffect.Invoke(effects[^1]);
            return effects[^1];
        }

        public IDisposable AddEffect(Effect.Effect p_effect, float p_time)
        {
            effects.Add(new(this, p_effect, p_time));
            onAddEffect.Invoke(effects[^1]);
            return effects[^1];
        }

        public void RemoveEffect(IDisposable p_effect)
        {
            if (typeof(StatefullEffect).IsAssignableFrom(p_effect.GetType()))
            {
                onRemoveEffect.Invoke((StatefullEffect)p_effect);
                effects.Remove((StatefullEffect)p_effect);
                p_effect.Dispose();
            }
        }

        /// <summary>
        /// Tick effects
        /// </summary>
        public virtual void Update()
        {            
            effects.ForEach  (effect => effect.Tick());
            effects.RemoveAll(effect => 
            {
                if (effect.state == EffectState.Expired)
                {
                    onRemoveEffect.Invoke(effect);
                    effect.Dispose();
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// Fixed tick effect
        /// </summary>
        public virtual void FixedUpdate()
        {
            effects.ForEach(effect => effect.FixedTick());
        }

        private void OnTriggerEnter(Collider p_collider)
        {
            onCollide.Invoke(p_collider);
        }
    }   
}