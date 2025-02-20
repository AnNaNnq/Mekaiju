using Mekaiju.AI;
using Mekaiju.Attributes;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Kaiju/Attack")]
    public class KaijuAttack : ScriptableObject
    {
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeReference, SubclassPicker]
        public IAttack Attack { get; private set; }

        private void OnValidate()
        {
            if (Attack == null)
                Debug.LogWarning("You must provide an attack for each kaiju attack.");
        }
    }
}
