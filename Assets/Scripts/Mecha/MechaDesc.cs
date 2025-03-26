using UnityEngine;
using Mekaiju.Utils;
using MyBox;
using Mekaiju.Entity;

namespace Mekaiju
{  

    /// <summary>
    /// Provide a description for a mecha
    /// </summary>
    [CreateAssetMenu(fileName = "New Mecha", menuName = "Mecha/Mecha")]
    public class MechaDesc : ScriptableObject
    {
        /// <summary>
        /// The max stamina handled by the mecha
        /// </summary>
        [field: Foldout("Statistics")]
        [field: SerializeField]
        public float stamina { get; private set; }

        /// <summary>
        /// The default stats of the mecha
        /// </summary>
        [field: SerializeField]
        public EnumArray<StatisticKind, IStatistic> statistics { get; private set; }

        /// <summary>
        /// The treshold to alter ability (base on part health)
        /// </summary>
        [field: OverrideLabel("Part Treshold (%)")]
        [field: SerializeField]
        public float partTreshold { get; private set; }

        /// <summary>
        /// The description for each parts
        /// </summary>
        [field: Foldout("General")]
        [field: SerializeField]
        public EnumArray<MechaPart, MechaPartDesc> parts { get; private set; }

        /// <summary>
        /// The description of standalone abilities
        /// </summary>
        [field: SerializeField]
        public EnumArray<StandaloneAbility, Ability> standalones { get; private set; }
    }
    
}
