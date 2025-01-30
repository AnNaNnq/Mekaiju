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
        public abstract IEnumerator Execute(MechaInstance p_self, BasicAI p_target);
    }

}
