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
            _defenseMod = p_self.modifiers[Statistics.Defense].Add(_defensePercentage, ModifierKind.Flat);
        }

        public override void OnRemove(EntityInstance p_self)
        {
            p_self.modifiers[Statistics.Defense].Remove(_defenseMod);
        }
    }
}
