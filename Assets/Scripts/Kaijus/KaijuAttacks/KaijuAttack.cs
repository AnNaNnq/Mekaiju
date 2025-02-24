using Mekaiju.Attributes;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Kaiju/Attack")]
    public class KaijuAttack : ScriptableObject
    {
        private static int nextId = 1;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public string name { get; private set; }

        [SerializeField, ReadOnly] // ReadOnly n�cessite un attribut personnalis� pour �tre visible dans l'Inspector
        private int animId;

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

        private void OnEnable()
        {
            // Si l'ID est � 0 (non initialis�), on lui assigne un nouvel ID
            if (animId == 0)
            {
                animId = ++nextId;
            }
        }

        public int Id => animId; // Propri�t� en lecture seule
    }
}
