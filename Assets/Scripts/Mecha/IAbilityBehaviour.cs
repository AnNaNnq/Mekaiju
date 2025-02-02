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
        /// 
        /// </summary>
        /// <param name="p_self"></param>
        /// <param name="p_target"></param>
        /// <returns></returns>
        public virtual IEnumerator Execute(MechaInstance p_self, BasicAI p_target)
        {
            return Execute(p_self, p_target, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_self"></param>
        /// <param name="p_target"></param>
        /// <returns></returns>
        public abstract IEnumerator Execute(MechaInstance p_self, BasicAI p_target, object p_opt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_opt"></param>
        /// <returns></returns>
        public abstract float Consumption(object p_opt);
    }

}
