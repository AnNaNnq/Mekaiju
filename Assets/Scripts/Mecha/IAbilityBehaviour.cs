using System.Collections;

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
        /// <param name="p_target"></param>
        /// <returns></returns>
        public abstract IEnumerator Execute(MechaInstance p_self, MechaInstance p_target);
    }

}
