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
        /// Store a reference to the parent.
        /// </summary>
        public EntityInstance parent { get; protected set; } = null;

        /// <summary>
        /// Store effect to apply to the current entity.
        /// </summary>
        public virtual List<StatefullEffect> effects { get; } = new();

        /// <summary>
        /// Allow time point tracking
        /// Can be overridden if entity is controlled by top level one.
        /// </summary>
        public virtual EnumArray<TimePoint, float> timePoints { get; } = new(() => float.MinValue);

        /// <summary>
        /// Define usefull states on an entity
        /// </summary>
        public virtual EnumArray<StateKind, State<bool>> states { get; } = new(() => new(false));

        /// <summary>
        /// Bind base entity stats.
        /// Must be overrided to use computedStats.
        /// </summary>
        public virtual EnumArray<StatisticKind, IStatistic> statistics { get; protected set; }

        /// <summary>
        /// Used to apply modifer on statistics.
        /// </summary>
        public virtual EnumArray<StatisticKind, ModifierCollection> modifiers { get; } = new(() => new());

        /// <summary>
        /// Compute stats with modifiers.
        /// </summary>
        /// <param name="p_kind">The targeted statistics.</param>
        /// <returns>The computed statistic.</returns>

        public virtual UnityEvent<IDamageable, float, DamageKind> onBeforeTakeDamage { get; } = new();
        public virtual UnityEvent<IDamageable, float, DamageKind> onAfterTakeDamage  { get; } = new();
        public virtual UnityEvent<float> onDealDamage { get; } = new();

        public UnityEvent<Collider> onCollide = new();
        
        public abstract bool isAlive    { get; }
        public abstract float health     { get; }
        public abstract float baseHealth { get; }

        public abstract float Heal      (float p_amount);
        public abstract float TakeDamage(IDamageable p_from, float p_damage, DamageKind p_kind);

        public virtual float baseStamina => 0f;
        public virtual float stamina     => 0f;

        public virtual void ConsumeStamina(float p_amount) {}
        public virtual void RestoreStamina(float p_amount) {}

        public virtual UnityEvent<StatefullEffect> onAddEffect    { get; } = new();
        public virtual UnityEvent<StatefullEffect> onRemoveEffect { get; } = new();

        public IDisposable AddEffect(Effect.Effect p_effect)
        {
            return AddEffect(p_effect, -1);
        }

        public IDisposable AddEffect(Effect.Effect p_effect, float p_time)
        {
            StatefullEffect t_ret = effects.Find(t_effect => t_effect.effect == p_effect);
            if (t_ret == null)
            {
                effects.Add(new(this, p_effect, p_time));
                t_ret = effects[^1];
                onAddEffect.Invoke(t_ret);
            }
            else
            {
                t_ret.Reset(p_time);
            }
            return t_ret;
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