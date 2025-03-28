using Mekaiju.Attributes;
using UnityEngine;

namespace Mekaiju.Tuto
{
    [CreateAssetMenu(fileName = "Step", menuName = "TutoStep")]
    public class TutoStep : ScriptableObject
    {

        [field: SerializeReference, SubclassPicker]
        public ITutorial behavior { get; private set; }
    }
}
