using System.Collections;
using Mekaiju.AI.Body;
using Mekaiju.Entity;
using MyBox;
using UnityEngine;

namespace Mekaiju
{
    public class MegaShieldAbility : IAbilityBehaviour
    {
        [Header("General")]
        [SerializeField]
        [OverrideLabel("Defense (%)")]
        private float _defense;

        [SerializeField]
        [OverrideLabel("Time (s)")]
        private float _time;

        [SerializeField]
        private float _consumption;

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return base.IsAvailable(p_self, p_opt) && p_self.stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                state = AbilityState.Active;
                p_self.ConsumeStamina(_consumption);

                var t_mod = p_self.modifiers[StatisticKind.Defense].Add(_defense / 100f, ModifierKind.Flat);   
                yield return new WaitForSeconds(_time);
                p_self.modifiers[StatisticKind.Defense].Remove(t_mod);

                state = AbilityState.Ready;
            }
        }
    }
}
