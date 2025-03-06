using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju.Entity
{
    public interface IDamageable
    {
        /// <summary>
        /// Must be invoke in TakeDamage implementation
        /// </summary>
        public UnityEvent<float> onTakeDamage { get; }

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
        public void Heal(float p_amount);

        /// <summary>
        /// Inflicts damage on the entity, reducing its health points.
        /// </summary>
        /// <param name="p_damage">The amount of damage to deal.</param>
        public void TakeDamage(float p_damage);
    }
}
