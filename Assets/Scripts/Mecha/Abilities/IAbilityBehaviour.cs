using System;
using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;

namespace Mekaiju
{

    /// <summary>
    /// An interface that defines all behaviour about ability.
    /// </summary>
    public abstract class IAbilityBehaviour
    {
        /// <summary>
        /// Called when capacity is loaded on a <see cref="MechaPartInstance"/>.
        /// </summary>
        /// <param name="p_self">The instance where the ability is loaded.</param>
        public virtual void Initialize(MechaPartInstance p_self) 
        {
            
        }

        /// <summary>
        /// Indicates whether the capacity can be triggered.
        /// </summary>
        /// <param name="p_self">The instance where the ability is loaded.</param>
        /// <param name="p_opt">An optional parameter (should be null if not needed).</param>
        /// <returns>true if capacity is able to be triggered, else false.</returns>
        public abstract bool IsAvailable(MechaPartInstance p_self, object p_opt);

        /// <summary>
        /// Simple overload of <see cref="Trigger(MechaPartInstance,BodyPartObject,object)"/>.
        /// </summary>
        /// <param name="p_self">The instance where the ability is loaded.</param>
        /// <param name="p_target">The enemy part that is locked (or null).</param>
        public virtual IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target)
        {
            return Trigger(p_self, p_target, null);
        }

        /// <summary>
        /// Must be called whenever you want to use this ability.<br/>
        /// Ensure ability is available before trigerring it (see <see cref="IsAvailable(MechaPartInstance, object)"/>).
        /// </summary>
        /// <param name="p_self">The instance where the ability is loaded.</param>
        /// <param name="p_target">The enemy part that is locked (or null).</param>
        /// <param name="p_opt">An optional parameter (should be null if not needed).</param>
        public abstract IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt);

        /// <summary>
        /// Use to release an ability if nessacary.<br/>
        /// Utils for helded ability.
        /// </summary>
        public virtual void Release()
        {

        }

        /// <summary>
        /// Must be called in <see cref="MonoBehviour.Update"/> to allow some common process.
        /// </summary>
        /// <param name="p_self">The instance where the ability is loaded.</param>
        public virtual void Tick(MechaPartInstance p_self)
        {

        }

        /// <summary>
        /// Must be called in <see cref="MonoBehviour.FixedUpdate"/> to allow some physics process.
        /// </summary>
        /// <param name="p_self">The instance where the ability is loaded.</param>
        public virtual void FixedTick(MechaPartInstance p_self)
        {

        }
    }

}
