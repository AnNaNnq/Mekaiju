using System;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MechaPartConfig
    {
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public MechaPartDesc Base { get; private set; }  

        /// <summary>
        /// 
        /// </summary>
        public Ability Special { get; private set; }

        public MechaPartConfig(MechaPartDesc p_desc, Ability p_special)
        {
            Base    = p_desc;
            Special = p_special;
        }
    }   

}
