using System;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public enum LegsSelector
    {
        Dash, Jump
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class LegsCompoundAbility : CompoundAbility<LegsSelector>
    {
        
    }
}
