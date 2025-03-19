using System;
using System.Collections;
using Mekaiju.AI.Body;
using Mekaiju.Entity;
using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju
{
    public class ReflectDamageAbility : IAbilityBehaviour
    {
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

                state = AbilityState.Ready;
            }
        }
    }
}
