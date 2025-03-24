using Mekaiju.Attributes;
using UnityEngine;
using UnityEngine.UI;


namespace Mekaiju.Entity.Effect
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
        [field: SerializeField]
        public Sprite[] effectImages = new Sprite[4];


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