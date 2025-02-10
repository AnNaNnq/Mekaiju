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
        /// <param name="p_self"></param>
        public virtual void Initialize(MechaPartInstance p_self) 
        {
            
        }

        /// <summary>
        /// Simple overload of <see cref="Trigger(MechaPartInstance,BodyPartObject,object)"/>
        /// </summary>
        /// <param name="p_self"></param>
        /// <param name="p_target"></param>
        /// <returns></returns>
        public virtual IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target)
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
        public abstract IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt);

        /// <summary>
        /// Must be called in <see cref="MonoBehviour.Update"/> to allow some common process.
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void Tick(MechaPartInstance p_self)
        {

        }

        /// <summary>
        /// Must be called in <see cref="MonoBehviour.FixedUpdate"/> to allow some physics process.
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void FixedTick(MechaPartInstance p_self)
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
