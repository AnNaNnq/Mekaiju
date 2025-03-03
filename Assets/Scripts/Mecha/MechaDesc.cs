using UnityEngine;
using Mekaiju.Utils;
using MyBox;

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
        /// The damage stat (ability must use this one to compute their damage)
        /// </summary>
        [field: SerializeField]
        public float damage { get; private set; }

        /// <summary>
        /// The defense stat (all damage taken must take care of this stat)
        /// </summary>
        [field: SerializeField]
        public float defense { get; private set; }

        /// <summary>
        /// The speed stat (all movement behaviour must use this one to compute their speed)
        /// </summary>
        [field: SerializeField]
        public float speed { get; private set; }

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
