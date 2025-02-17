using UnityEngine;
using Mekaiju.Utils;

namespace Mekaiju
{  

    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "New Mecha", menuName = "Mecha/Mecha")]
    public class MechaDesc : ScriptableObject
    {
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public float stamina { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public GameObject prefab { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public EnumArray<MechaPart, MechaPartDesc> parts { get; private set; }
    }
    
}
