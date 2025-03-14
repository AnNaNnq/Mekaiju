using System;
using UnityEngine;

namespace Mekaiju
{
    [Serializable]
    public abstract class AbilityGranter
    {
        public abstract void Grant(MechaDesc p_desc, Ability p_ability);   
    }
}
