using Mekaiju.Attributes;
using UnityEngine;

namespace Mekaiju
{

    [CreateAssetMenu(fileName = "New Ability", menuName = "Mecha/Ability")]
    public class Ability : ScriptableObject
    {
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public string Description { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public GameObject Prefab { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public MechaPart Target { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeReference, SubclassPicker]
        public IAbilityBehaviour Behaviour { get; private set; }
    }
    
}
