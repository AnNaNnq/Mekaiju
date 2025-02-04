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
        public float Health { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public float Stamina { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public GameObject Prefab { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public EnumArray<MechaPart, MechaPartDesc> Parts { get; private set; }
    }
    
}
