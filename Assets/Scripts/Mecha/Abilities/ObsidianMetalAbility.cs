using System;
using System.Collections;
using Mekaiju.AI.Body;
using Mekaiju.Entity;
using UnityEngine;

namespace Mekaiju
{
    public class ObsidianMetalAbility : IAbilityBehaviour
    {
        [SerializeField]
        private float _healthBuff;

        public override void Initialize(EntityInstance p_self)
        {
            p_self.modifiers[StatisticKind.Health].Add(_healthBuff, ModifierKind.Flat);
        }
    }
}
