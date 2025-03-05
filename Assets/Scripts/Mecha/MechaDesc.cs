using UnityEngine;
using Mekaiju.Utils;
using MyBox;
using Mekaiju.Entity;

namespace Mekaiju
{  

    /// <summary>
    /// Provide a description for a mecha
    /// </summary>
    [CreateAssetMenu(fileName = "New Mecha", menuName = "Mecha/Mecha")]
    public class MechaDesc : ScriptableObject
    {
        /// <summary>
        /// The max stamina handled by the mecha
        /// </summary>
        [field: Foldout("Statistics")]
        [field: SerializeField]
        public float stamina { get; private set; }

        /// <summary>
        /// The default stats of the mecha
        /// </summary>
        public EnumArray<Statistics, float> statistics;

        /// <summary>
        /// The mecha prefab/mesh
        /// </summary>
        [field: Foldout("General")]
        [field: SerializeField]
        public GameObject prefab { get; private set; }

        /// <summary>
        /// The description for each parts
        /// </summary>
        [field: SerializeField]
        public EnumArray<MechaPart, MechaPartDesc> parts { get; private set; }
    }
    
}
