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
        /// Called when effect is added to a <see cref="IEntityInstance"/> 
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void OnAdd(IEntityInstance p_self)
        {

        }

        /// <summary>
        /// Must be called in <see cref="MonoBehviour.Update"/> to allow some common process.
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void Tick(IEntityInstance p_self)
        {

        }

        /// <summary>
        /// Must be called in <see cref="MonoBehviour.FixedUpdate"/> to allow some physics process.
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void FixedTick(IEntityInstance p_self)
        {

        }

        /// <summary>
        /// Called when effect is removed from <see cref="IEntityInstance"/> 
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void OnRemove(IEntityInstance p_self)
        {

        }
    }

}
