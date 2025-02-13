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

        public void OnValidate()
        {
            if (!Prefab)
                Debug.LogWarning("You must provide a prefab for each mecha ability.");

            if (Behaviour == null)
            {
                Debug.LogWarning("You must provide a behaviour for each mecha ability.");
            }
            else
            {
                if (typeof(ICompoundAbility).IsAssignableFrom(Behaviour.GetType()))
                {
                    (Behaviour as ICompoundAbility).CheckAbilityLoop(this);
                }
            }
        }
    }
    
}
