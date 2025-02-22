using Mekaiju.Attributes;
using UnityEngine;

namespace Mekaiju
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Entity/Effect")]
    public class Effect : ScriptableObject
    {   
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public string description { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeReference, SubclassPicker]
        public IEffectBehaviour behaviour { get; private set; }

        private void OnValidate()
        {
            if (behaviour == null)
            {
                Debug.LogWarning("You must provide a behaviour for each mecha effect.");
            }
        }
    }
}