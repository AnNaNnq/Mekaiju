using System.Collections;
using System.Collections.Generic;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.Entity;
using Mekaiju.Entity.Effect;
using MyBox;
using UnityEngine;

namespace Mekaiju
{
    public class StunAbility : IAbilityBehaviour
    {
        [Header("General")]
        [SerializeField]
        [Tooltip("The max reach of the ability (in m)")]
        [OverrideLabel("Max Reach (m)")]
        private float _maxReach;

        [SerializeField]
        [Tooltip("Used to compute stun effect time (eg. dst = 10, fct = .5 => t = 10 * .5 = 5s)")]
        private float _distanceFactor;

        [SerializeField]
        private float _consumption;

        [Header("Other")]
        [SerializeField]
        private Effect _stunEffect;

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

                List<KaijuInstance> t_stunned = new();

                Collider[] t_hitted = Physics.OverlapSphere(p_self.parent.transform.position, _maxReach);
                foreach (var t_hit in t_hitted)
                {
                    if (t_hit.attachedRigidbody)
                    {
                        if (t_hit.attachedRigidbody.gameObject.TryGetComponent<KaijuInstance>(out var t_inst))
                        {
                            if (!t_stunned.Contains(t_inst))
                            {
                                t_stunned.Add(t_inst);
                                t_inst.AddEffect(_stunEffect, (_maxReach - Vector3.Distance(p_self.parent.transform.position, t_hit.gameObject.transform.position)) * _distanceFactor);
                            }
                        }
                    }
                }

                state = AbilityState.Ready;
            }
            yield return null;
        }
    }
}
