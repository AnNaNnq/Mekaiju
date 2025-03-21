using Mekaiju.AI.PhaseAttack;
using Mekaiju.Attributes;
using UnityEngine;

namespace Mekaiju.AI.Objet
{
    [CreateAssetMenu(fileName = "New Phase Attack", menuName = "Kaiju/Phase Attack")]
    public class KaijuPhaseAttack : ScriptableObject
    {
        [field: SerializeReference, SubclassPicker]
        public PhaseAttack.PhaseAttack attack { get; private set; }

        private void OnValidate()
        {
            if (attack == null)
                Debug.LogWarning("You must provide an passive for each kaiju passive.");
        }
    }
}
