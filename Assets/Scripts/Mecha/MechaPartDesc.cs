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
        public Ability DefaultAbility { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public Ability SpecialAbility { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasSpecial => !SpecialAbility.IsUnityNull();
    }

}
