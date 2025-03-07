using UnityEngine;

namespace Mekaiju.Entity.Effect
{
    public class ProtectionEffect : IEffectBehaviour
    {
        [SerializeField]
        private int _defensePercentage;
        private Modifier _defenseMod;

        public override void OnAdd(EntityInstance p_self)
        {
            _defenseMod = p_self.modifiers[ModifierTarget.Defense].Add(_defensePercentage);
            Debug.Log("ProtectionEffect: OnAdd");
        }

        public override void OnRemove(EntityInstance p_self)
        {
            p_self.modifiers[ModifierTarget.Defense].Remove(_defenseMod);
        }
    }
}
