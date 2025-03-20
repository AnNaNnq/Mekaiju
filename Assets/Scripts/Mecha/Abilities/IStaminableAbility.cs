using Mekaiju;
using Mekaiju.Entity;
using UnityEngine;

namespace Mekaiju
{
    public abstract class IStaminableAbility : ICooldownAbility
    {
        /// <summary>
        /// The stamina consumed on ability trigger.
        /// </summary>
        [SerializeField]
        private float _consumption;

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return base.IsAvailable(p_self, p_opt) && p_self.stamina - _consumption >= 0f;
        }

        public void ConsumeStamina(EntityInstance p_self)
        {
            p_self.ConsumeStamina(_consumption);
        }
    }
}
