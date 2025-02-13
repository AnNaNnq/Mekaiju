using System;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class IEffectBehaviour
    {
        /// <summary>
        /// Called when effect is added to a <see cref="MechaInstance"/> 
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void OnAdd(MechaInstance p_self)
        {

        }

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
        /// Called when effect is removed from <see cref="MechaInstance"/> 
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void OnRemove(MechaInstance p_self)
        {

        }
    }

}
