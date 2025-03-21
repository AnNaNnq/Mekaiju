using System;

namespace Mekaiju.Entity.Effect
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class IEffectBehaviour
    {
        /// <summary>
        /// Called when effect is added to a <see cref="EntityInstance"/> 
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void OnAdd(EntityInstance p_self)
        {

        }

        /// <summary>
        /// Must be called in <see cref="MonoBehviour.Update"/> to allow some common process.
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void Tick(EntityInstance p_self)
        {

        }

        /// <summary>
        /// Must be called in <see cref="MonoBehviour.FixedUpdate"/> to allow some physics process.
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void FixedTick(EntityInstance p_self)
        {

        }

        /// <summary>
        /// Called when effect is removed from <see cref="EntityInstance"/> 
        /// </summary>
        /// <param name="p_self"></param>
        public virtual void OnRemove(EntityInstance p_self)
        {

        }
    }

}
