using UnityEngine;

namespace Mekaiju.Entity
{
    /// <summary>
    /// Some entity states
    /// </summary>
    public enum State
    {
        // Define if entity is controlled by third party
        MovementOverrided,

        // Prevent any movement
        MovementLocked,

        // Prevent the usage of any ability
        AbilityLocked,

        // Define if entity is protected (ie shield...)
        Protected,

        // Define if entity is grounded
        Grounded,
    }
}
