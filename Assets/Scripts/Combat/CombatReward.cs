using System.Collections.Generic;
using UnityEngine;

namespace Mekaiju
{
    [CreateAssetMenu(fileName = "new Combat Reward", menuName = "Combat/Reward")]
    public class CombatReward : ScriptableObject
    {
        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public List<Ability> abilities { get; private set; }
    }
}
