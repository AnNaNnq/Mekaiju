using System.Collections;
using Mekaiju.AI;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class IAbilityBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual void Initialize() 
        {
            
        }

        /// <summary>
        /// Simple overload of <see cref="Trigger(MechaInstance,BasicAI,object)"/>
        /// </summary>
        /// <param name="p_self"></param>
        /// <param name="p_target"></param>
        /// <returns></returns>
        public virtual IEnumerator Trigger(MechaInstance p_self, BasicAI p_target)
        {
            return Trigger(p_self, p_target, null);
        }

        /// <summary>
        /// Must be called whenever you want to use this ability.<br/>
        /// Ensure that you have enough stamina (see <see cref="Consumption"/>)
        /// </summary>
        /// <param name="p_self"></param>
        /// <param name="p_target"></param>
        /// <returns></returns>
        public abstract IEnumerator Trigger(MechaInstance p_self, BasicAI p_target, object p_opt);

        /// <summary>
        /// Must be called in <see cref="MonoBehviour.Update"/> to allow some common process.
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void Tick(MechaInstance p_self)
        {

        }

        /// <summary>
        /// Must be called in <see cref="MonoBehviour.FixedUpdate"/> to allow some physics process.
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void FixedTick(MechaInstance p_self)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_opt"></param>
        /// <returns></returns>
        public abstract float Consumption(object p_opt);
    }

}
