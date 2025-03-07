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
        protected EnumArray<Statistics, float> statistics { get; }

        /// <summary>
        /// Used to apply modifer on statistics.
        /// </summary>
        public EnumArray<Statistics, ModifierCollection> modifiers { get; }

        /// <summary>
        /// Compute stats with modifiers.
        /// </summary>
        /// <param name="p_kind">The targeted statistics.</param>
        /// <returns>The computed statistic.</returns>
        public float ComputedStatistics(Statistics p_kind);
    }
}
