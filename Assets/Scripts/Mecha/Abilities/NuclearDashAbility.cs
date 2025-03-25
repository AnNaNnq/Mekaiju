using System.Collections;
using Mekaiju.AI.Body;
using Mekaiju.Entity;
using MyBox;
using UnityEngine;

namespace Mekaiju
{
    public class NuclearDashAbility : IAbilityBehaviour
    {
        [Header("General")]
        [SerializeField]
        private DashAlterPayload _dashFactor;

        [SerializeField]
        [OverrideLabel("Duration (s)")]
        private float _duration;

        [SerializeField]
        [OverrideLabel("Cooldown (s)")]
        private float _cooldown;

        [SerializeField]
        private float _consumption;

        [Header("Other")]
        [SerializeField]
        private Ability _dashAbility;

        private DashAlterPayload _reset;
        private float             _currentCooldown;

        public override float cooldown => _currentCooldown;

        public override void Initialize(EntityInstance p_self)
        {
            base.Initialize(p_self);

            _reset = new();
            _reset.forceFactor    = 1f;
            _reset.durationFactor = 1f;

            _currentCooldown = 0;
        }

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return base.IsAvailable(p_self, p_opt) && p_self.stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                state.Set(AbilityState.Active);

                p_self.ConsumeStamina(_consumption);

                _dashAbility.behaviour.Alter(_dashFactor);
                yield return new WaitForSeconds(_duration);
                _dashAbility.behaviour.Alter(_reset);

                state.Set(AbilityState.InCooldown);

                _currentCooldown = _cooldown;
                yield return new WaitUntil(() => (_currentCooldown -= Time.deltaTime) <= 0);

                state.Set(AbilityState.Ready);
            }
        }
    }
}