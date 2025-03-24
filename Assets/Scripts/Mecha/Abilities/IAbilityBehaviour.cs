using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.Entity;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    public enum AbilityState
    {
        Ready, InCooldown, Active
    }

    public abstract class IPayload {}
    public interface IAlteration {}

    public class Alteration<T> : IAlteration
    {
        public T payload;
        public T diff;

        public Alteration(T p_payload, T p_diff)
        {
            payload = p_payload;
            diff    = p_diff;
        }
    }


    /// <summary>
    /// An interface that defines all behaviour about ability.
    /// </summary>
    public abstract class IAbilityBehaviour
    {
        /// <summary>
        /// Return the ability state.
        /// </summary>
        public AbilityState state { get; protected set; }

        /// <summary>
        /// Return the remaining cooldown time
        /// </summary>
        public virtual float cooldown { get => 0f; }

        /// <summary>
        /// Called when capacity is loaded on a <see cref="EntityInstance"/>.
        /// </summary>
        /// <param name="p_self">The instance where the ability is loaded.</param>
        public virtual void Initialize(EntityInstance p_self) 
        {
            state = AbilityState.Ready;
        }

        /// <summary>
        /// Used to handle alter payload.
        /// </summary>
        /// <param name="p_payload">The data used to alter ability.</param>
        public virtual IAlteration Alter<T>(T p_payload) { return null; }

        /// <summary>
        /// Used to restore default ability properties.
        /// </summary>
        public virtual void Revert(IAlteration p_alteration) {}

        /// <summary>
        /// Indicates whether the capacity can be triggered.
        /// </summary>
        /// <param name="p_self">The instance where the ability is loaded.</param>
        /// <param name="p_opt">An optional parameter (should be null if not needed).</param>
        /// <returns>true if capacity is able to be triggered, else false.</returns>
        public virtual bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return state == AbilityState.Ready && !p_self.states[StateKind.AbilityLocked];
        }

        /// <summary>
        /// Simple overload of <see cref="Trigger(EntityInstance,BodyPartObject,object)"/>.
        /// </summary>
        /// <param name="p_self">The instance where the ability is loaded.</param>
        /// <param name="p_target">The enemy part that is locked (or null).</param>
        public virtual IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target)
        {
            return Trigger(p_self, p_target, null);
        }

        /// <summary>
        /// Must be called whenever you want to use this ability.<br/>
        /// Ensure ability is available before trigerring it (see <see cref="IsAvailable(EntityInstance, object)"/>).
        /// </summary>
        /// <param name="p_self">The instance where the ability is loaded.</param>
        /// <param name="p_target">The enemy part that is locked (or null).</param>
        /// <param name="p_opt">An optional parameter (should be null if not needed).</param>
        public virtual IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            yield return null;
        }

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
        public virtual void Tick(EntityInstance p_self)
        {

        }

        /// <summary>
        /// Must be called in <see cref="MonoBehviour.FixedUpdate"/> to allow some physics process.
        /// </summary>
        /// <param name="p_self">The instance where the ability is loaded.</param>
        public virtual void FixedTick(EntityInstance p_self)
        {

        }
    }

}
