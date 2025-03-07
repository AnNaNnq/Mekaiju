using Mekaiju.AI.Passive;
using Mekaiju.Attributes;
using UnityEngine;

namespace Mekaiju.AI.Objet
{
    [CreateAssetMenu(fileName = "New Passive", menuName = "Kaiju/Passive")]
    public class KaijuPassive : ScriptableObject
    {
        [field: SerializeReference, SubclassPicker]
        public Passive.Passive passive { get; private set; }

        private void OnValidate()
        {
            if (passive == null)
                Debug.LogWarning("You must provide an passive for each kaiju passive.");
        }
    }
}
