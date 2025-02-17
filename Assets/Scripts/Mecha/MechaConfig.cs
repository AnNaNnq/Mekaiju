using Mekaiju.Utils;
using UnityEngine;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    public class MechaConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public MechaDesc desc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public EnumArray<MechaPart, MechaPartConfig> parts { get; private set; }

        public MechaConfig(MechaDesc p_desc)
        {
            desc  = p_desc;
            parts = p_desc.Parts.Select((t_key, t_desc) => new MechaPartConfig(t_desc));
        }
    }
}

