using Mekaiju.Attributes;
using MyBox;
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
        public string description { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField, Tooltip("Assiciated GameObeject name in mecha mesh tree")]
        public string objectName { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public MechaPart target { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public Sprite icon { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeReference, SubclassPicker]
        public IAbilityBehaviour behaviour { get; private set; }

        public void OnValidate()
        {
            if (behaviour == null)
            {
                Debug.LogWarning("You must provide a behaviour for each mecha ability.");
            }
            else
            {
                if (typeof(ICompoundAbility).IsAssignableFrom(behaviour.GetType()))
                {
                    (behaviour as ICompoundAbility).CheckAbilityLoop(this);
                }
            }
        }
    }
    
}
