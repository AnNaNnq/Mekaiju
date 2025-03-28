using Mekaiju.Utils;
using UnityEngine;

namespace Mekaiju.Entity
{
    public interface IModifiable
    {
        /// <summary>
        /// Bind base entity stats.
        /// Must be overrided to use computedStats.
        /// </summary>
        public EnumArray<StatisticKind, IStatistic> statistics { get; protected set; }

        /// <summary>
        /// Used to apply modifer on statistics.
        /// </summary>
        public EnumArray<StatisticKind, ModifierCollection> modifiers { get; }
    }
}
