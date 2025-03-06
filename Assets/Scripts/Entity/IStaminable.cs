using UnityEngine;

namespace Mekaiju.Entity
{
    public interface IStaminable
    {
        /// <summary>
        /// Gets the base stamina of the entity.
        /// Can be overridden if stamina management is required.
        /// </summary>
        public float baseStamina { get; }

        /// <summary>
        /// The current stamina of the entity
        /// </summary>
        public float stamina { get; }

        /// <summary>
        /// Reduces the entity's stamina by consuming a specified amount.
        /// Can be overridden if stamina management is required.
        /// </summary>
        /// <param name="p_amount">The amount of stamina to consume.</param>
        public void ConsumeStamina(float p_amount);

        /// <summary>
        /// Restores the entity's stamina by a specified amount.
        /// Can be overridden if stamina management is required.
        /// </summary>
        /// <param name="p_amount">The amount of stamina to restore.</param>
        public void RestoreStamina(float p_amount);
    }
}
