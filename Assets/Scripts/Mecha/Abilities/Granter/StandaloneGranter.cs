using UnityEngine;

namespace Mekaiju
{
    public class StandaloneGranter : AbilityGranter
    {
        [SerializeField]
        private StandaloneAbility _target;

        public override void Grant(MechaDesc p_desc, Ability p_ability)
        {
            p_desc.standalones[_target] = p_ability;
        }
    }
}
