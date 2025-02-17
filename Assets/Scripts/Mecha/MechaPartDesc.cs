using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MechaPartDesc
    {
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public float Health { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public Ability Ability { get; private set; }
    }

}
