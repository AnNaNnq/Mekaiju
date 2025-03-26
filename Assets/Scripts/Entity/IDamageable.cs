using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju.Entity
{
    public enum DamageKind
    {
        // Damage dealt by another Entity
        Direct,

        // Damage dealt by third party non Entity
        Indirect,
    }

    public interface IDamageable
    {
        /// <summary>
        /// Must be invoke before any IDamageable took damage.<br/>
        /// 0: The IDamageable attempting to deal damage. Should be null if 2 is Indirect.<br/>
        /// 1: The damage that the IDamageable intends to deal.<br/>
        /// 2: The type of damage being dealt
        /// </summary>
        public UnityEvent<IDamageable, float, DamageKind> onBeforeTakeDamage { get; }

        /// <summary>
        /// Must be invoke after any IDamageable took damage.<br/>
        /// 0: The IDamageable attempting to deal damage. Should be null if 2 is Indirect.<br/>
        /// 1: The real damages taken.<br/>
        /// 2: The type of damage being dealt
        /// </summary>
        public UnityEvent<IDamageable, float, DamageKind> onAfterTakeDamage { get; }

        /// <summary>
        /// 
        /// </summary>
        public UnityEvent<float> onDealDamage { get; }

        /// <summary>
        /// Return if entity is alive
        /// </summary>
        public bool isAlive { get; }

        /// <summary>
        /// The current health of the entity.
        /// </summary>
        public float health { get; }

        /// <summary>
        /// Gets the base health of the entity.
        /// </summary>
        public float baseHealth { get; }

        /// <summary>
        /// Heals the entity by restoring a specified amount of health points.
        /// </summary>
        /// <param name="p_amount">The amount of health to restore.</param>
        /// <returns>The real amount of health restored.</returns>
        public float Heal(float p_amount);

        /// <summary>
        /// Inflicts damage on the entity, reducing its health points.
        /// </summary>
        /// <param name="p_from">The source of damage. Should be null if p_kind is Indirect</param>
        /// <param name="p_damage">The amount of damage to deal.</param>
        /// <param name="p_kind">The type of damage being dealt.</param>
        /// <returns>The real damage taken.</returns>
        public float TakeDamage(IDamageable p_from, float p_damage, DamageKind p_kind);
    }
}
