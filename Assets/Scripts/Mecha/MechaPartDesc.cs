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
        /// The percent of total mecha health
        /// </summary>
        [field: SerializeField, Range(0f, 1f)]
        public float healthPercent { get; private set; }

        /// <summary>
        /// Specify if ability can be replaced
        /// </summary>
        [field: SerializeField, Tooltip("Specify whether the ability can be replaced by a recoverable one")]
        public bool isAbilityReplaceable { get; private set; }

        /// <summary>
        /// The default ability
        /// </summary>
        [field: SerializeField]
        public Ability ability;
    }

}
