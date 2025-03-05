using Mekaiju.AI.Attack;
using Mekaiju.Attributes;
using UnityEngine;

namespace Mekaiju.AI.Objet
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Kaiju/Attack")]
    public class KaijuAttack : ScriptableObject
    {
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeReference, SubclassPicker]
        public IAttack attack { get; private set; }

        private void OnValidate()
        {
            if (attack == null)
                Debug.LogWarning("You must provide an attack for each kaiju attack.");
        }
    }
}
