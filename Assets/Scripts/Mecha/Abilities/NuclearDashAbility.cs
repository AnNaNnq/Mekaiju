using System.Collections;
using Mekaiju.AI.Body;
using Mekaiju.Entity;
using MyBox;
using UnityEngine;

namespace Mekaiju
{
    public class NuclearDashAbility : IStaminableAbility
    {
        [Header("General")]
        [SerializeField]
        [OverrideLabel("Alteration factor")]
        [Tooltip("force = .5 means force = (1 + .5) * force")]
        private DashPayload _dashFactor;

        [SerializeField]
        [OverrideLabel("Duration (s)")]
        private float _duration;

        [Header("Other")]
        [SerializeField]
        private Ability _dashAbility;

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return base.IsAvailable(p_self, p_opt);
        }

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                state = AbilityState.Active;

                ConsumeStamina(p_self);

                var t_alteration = _dashAbility.behaviour.Alter(_dashFactor);
                yield return new WaitForSeconds(_duration);
                _dashAbility.behaviour.Revert(t_alteration);

                yield return WaitForCooldown();

                state = AbilityState.Ready;
            }
        }
    }
}