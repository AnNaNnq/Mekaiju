using System;
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
        [field: SerializeField, Tooltip("Specify whether the ability can be replaced by a recoverable one")]
        public bool isAbilityReplaceable { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public Ability Ability { get; private set; }
    }

}
