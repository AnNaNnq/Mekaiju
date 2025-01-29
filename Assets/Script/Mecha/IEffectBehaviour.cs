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
        /// 
        /// </summary>
        public abstract void Tick(MechaInstance self);
    }

}
