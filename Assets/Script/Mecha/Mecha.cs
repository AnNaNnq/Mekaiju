using UnityEngine;

namespace Mekaiju
{  

    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "New Mecha", menuName = "Mecha/Mecha")]
    public class Mecha : ScriptableObject
    {
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public GameObject Prefab { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public Utils.EnumArray<MechaPart, MechaPartDesc> Parts { get; private set; }
    }
    
}
