using System;
using Mekaiju.Utils;

namespace Mekaiju
{

    [Serializable]
    public class MechaConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public Mecha Base { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public EnumArray<MechaPart, MechaPartConfig> Parts; 

        public MechaConfig(Mecha p_base)
        {
            Base  = p_base;
            Parts = p_base.Parts.Select(part => new MechaPartConfig(part, null));
        }
    }

}
