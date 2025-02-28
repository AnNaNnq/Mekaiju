using Mekaiju.Attributes;
using UnityEngine;

namespace Mekaiju.AI
{
    [CreateAssetMenu(fileName = "New Passive", menuName = "Kaiju/Passive")]
    public class KaijuPassive : ScriptableObject
    {
        [field: SerializeReference, SubclassPicker]
        public IPassive passive { get; private set; }

        private void OnValidate()
        {
            if (passive == null)
                Debug.LogWarning("You must provide an passive for each kaiju passive.");
        }
    }
}
