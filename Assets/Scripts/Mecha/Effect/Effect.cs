using Mekaiju.Attributes;
using UnityEngine;

namespace Mekaiju
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Mecha/Effect")]
    public class Effect : ScriptableObject
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
        [field: SerializeReference, SubclassPicker]
        public IEffectBehaviour Behaviour { get; private set; }

        private void OnValidate()
        {
            if (Behaviour == null)
                Debug.LogWarning("You must provide a behaviour for each mecha effect.");
        }
    }
}