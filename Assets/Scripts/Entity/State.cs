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

        // Prevent entity from moving
        Stun,

        // Define if entity is protected (ie shield...)
        Protected,

        // Define if entity is grounded
        Grounded,
    }
}
