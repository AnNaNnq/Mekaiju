using System.Collections;
using Mekaiju.AI.Body;
using Mekaiju.Entity;
using MyBox;
using UnityEngine;

namespace Mekaiju
{
    public class MegaShieldAbility : IStaminableAbility
    {
        [SerializeField]
        [OverrideLabel("Defense (%)")]
        private float _defense;

        [SerializeField]
        [OverrideLabel("Time (s)")]
        private float _time;

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                state = AbilityState.Active;

                ConsumeStamina(p_self);

                var t_mod = p_self.modifiers[StatisticKind.Defense].Add(_defense / 100f, ModifierKind.Flat);   
                yield return new WaitForSeconds(_time);
                p_self.modifiers[StatisticKind.Defense].Remove(t_mod);

                yield return WaitForCooldown();

                state = AbilityState.Ready;
            }
        }
    }
}
