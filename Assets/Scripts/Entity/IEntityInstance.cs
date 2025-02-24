using Mekaiju.Utils;
using UnityEngine;

namespace Mekaiju
{
    /// <summary>
    /// A base class for all entity instance
    /// Allow effects on them
    /// </summary>
    public abstract class IEntityInstance : MonoBehaviour
    {
        /// <summary>
        /// Allow time point tracking
        /// Can be overridden if entity is controlled by top level one.
        /// </summary>
        public virtual EnumArray<TimePoint, float> timePoints { get; } = new(() => float.MinValue);

        /// <summary>
        /// Allow buff/debuff effect
        /// </summary>
        public abstract EnumArray<ModifierTarget, ModifierCollection> modifiers { get; }

        /// <summary>
        /// Gets the base health of the entity.
        /// </summary>
        public abstract float baseHealth { get; }

        /// <summary>
        /// Heals the entity by restoring a specified amount of health points.
        /// </summary>
        /// <param name="p_amount">The amount of health to restore.</param>
        public abstract void Heal(float p_amount);

        /// <summary>
        /// Inflicts damage on the entity, reducing its health points.
        /// </summary>
        /// <param name="p_damage">The amount of damage to deal.</param>
        public abstract void TakeDamage(float p_damage);

        /// <summary>
        /// Gets the base stamina of the entity.
        /// Can be overridden if stamina management is required.
        /// </summary>
        public virtual float baseStamina { get => 0f; }

        /// <summary>
        /// Reduces the entity's stamina by consuming a specified amount.
        /// Can be overridden if stamina management is required.
        /// </summary>
        /// <param name="p_amount">The amount of stamina to consume.</param>
        public virtual void ConsumeStamina(float p_amount) {}

        /// <summary>
        /// Restores the entity's stamina by a specified amount.
        /// Can be overridden if stamina management is required.
        /// </summary>
        /// <param name="p_amount">The amount of stamina to restore.</param>
        public virtual void RestoreStamina(float p_amount) {}
    }   
}