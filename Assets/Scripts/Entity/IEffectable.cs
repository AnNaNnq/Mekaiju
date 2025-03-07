using System;
using Mekaiju.Entity.Effect;
using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju.Entity
{
    public interface IEffectable
    {
        /// <summary>
        /// Notify when an effect is added to the current entity.
        /// </summary>
        UnityEvent<StatefullEffect> onAddEffect { get; }

        /// <summary>
        /// Notify when an effect is removed from the current entity.
        /// </summary>
        UnityEvent<StatefullEffect> onRemoveEffect { get; }

        /// <summary>
        /// Adds a new effect to the list of active effects without a timeout. 
        /// The effect will remain active indefinitely until it is manually removed.
        /// </summary>
        /// <param name="p_effect">The effect to be added.</param>
        IDisposable AddEffect(Effect.Effect p_effect);

        /// <summary>
        /// Adds a new effect to the list of active effects, with a specified duration.
        /// </summary>
        /// <param name="p_effect">The effect to be added.</param>
        /// <param name="p_time">The duration of the effect in seconds.</param>
        IDisposable AddEffect(Effect.Effect p_effect, float p_time);

        /// <summary>
        /// Remove an effect.
        /// </summary>
        /// <param name="p_effect">The effect to remove.</param>
        void RemoveEffect(IDisposable p_effect);
    }
}