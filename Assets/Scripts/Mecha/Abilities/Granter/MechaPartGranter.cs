using System;
using UnityEngine;

namespace Mekaiju
{
    [Serializable]
    public class MechaPartGranter : AbilityGranter
    {
        [SerializeField]
        private MechaPart _part;

        public override void Grant(MechaDesc p_desc, Ability p_ability)
        {
            p_desc.parts[_part].ability = p_ability;
        }
    }
}
