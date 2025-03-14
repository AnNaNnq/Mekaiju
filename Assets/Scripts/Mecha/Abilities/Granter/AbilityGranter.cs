using System;
using UnityEngine;

namespace Mekaiju
{
    [Serializable]
    public abstract class AbilityGranter
    {
        [field: SerializeField]
        public string targetName { get; private set; }

        public abstract void Grant(MechaDesc p_desc, Ability p_ability);   
    }
}
