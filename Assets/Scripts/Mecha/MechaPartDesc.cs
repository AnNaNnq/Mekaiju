using System;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// Provide a description for a mecha part
    /// </summary>
    [Serializable]
    public class MechaPartDesc
    {
        /// <summary>
        /// Specify if ability can be replaced
        /// </summary>
        [field: SerializeField, Tooltip("Specify whether the ability can be replaced by a recoverable one")]
        public bool isAbilityReplaceable { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public MechaPart part { get; private set; }

        /// <summary>
        /// The default ability
        /// </summary>
        [field: SerializeField]
        public Ability ability;
    }

}
