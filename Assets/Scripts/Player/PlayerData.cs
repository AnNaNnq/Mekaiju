using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    public class PlayerData
    {
        /// <summary>
        /// 
        /// </summary>
        public MechaDesc Mecha;

        public PlayerData()
        {
            Mecha = Resources.LoadAll<MechaDesc>("")[0];
        }   
    }

}

