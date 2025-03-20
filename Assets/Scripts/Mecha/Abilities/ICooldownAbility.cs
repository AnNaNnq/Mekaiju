using System.Collections;
using Mekaiju.Entity;
using MyBox;
using UnityEngine;

namespace Mekaiju
{
    public abstract class ICooldownAbility : IAbilityBehaviour
    {
        /// <summary>
        /// The time to wait before ability began available.
        /// </summary>
        [Header("General")]
        [SerializeField]
        [OverrideLabel("Cooldown (s)")]
        private float _cooldown;

        /// <summary>
        /// Cooldown remaining time.
        /// </summary>
        private float _currentCooldown;

        public override float cooldown => _currentCooldown;

        public override void Initialize(EntityInstance p_self)
        {
            base.Initialize(p_self);
            _currentCooldown = 0;
        }

        public IEnumerator WaitForCooldown()
        {
            state = AbilityState.InCooldown;

            _currentCooldown = _cooldown;
            yield return new WaitUntil(() => (_currentCooldown -= Time.deltaTime) <= 0);
        }
    }
}
