using UnityEngine;

namespace Mekaiju.Entity.Effect
{
    public class ProtectionEffect : IEffectBehaviour
    {
        [SerializeField]
        private int _defensePercentage;
        private Modifier _defenseMod;

        public override void OnAdd(IEntityInstance p_self)
        {
            _defenseMod = p_self.modifiers[Statistics.Defense].Add(_defensePercentage);
            Debug.Log("ProtectionEffect: OnAdd");
        }

        public override void OnRemove(IEntityInstance p_self)
        {
            p_self.modifiers[Statistics.Defense].Remove(_defenseMod);
        }
    }
}
