using System;
using System.Collections;
using Mekaiju.AI.Body;
using Mekaiju.Entity;
using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju
{
    public class ReflectDamageAbility : IStaminableAbility
    {
        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                state.Set(AbilityState.Active);

                ConsumeStamina(p_self);

                bool t_done = false;

                UnityAction<IDamageable, float, DamageKind> t_cb = (t_from, t_damage, t_kind) => {
                    if (t_kind == DamageKind.Direct)
                    {
                        t_from.TakeDamage(p_self.parent, t_damage, DamageKind.Direct);
                        p_self.states[StateKind.Invulnerable].Set(true);
                        t_done = true;
                    }
                };

                p_self.onBeforeTakeDamage.AddListener(t_cb);

                yield return new WaitUntil(() => t_done);

                p_self.onBeforeTakeDamage.RemoveListener(t_cb);

                yield return new WaitForEndOfFrame();

                p_self.states[StateKind.Invulnerable].Set(false);

                yield return WaitForCooldown();

                state.Set(AbilityState.Ready);
            }
        }
    }
}
