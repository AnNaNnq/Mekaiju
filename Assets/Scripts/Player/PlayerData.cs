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
        public MechaConfig mechaConfig;

        public PlayerData()
        {
            mechaConfig = new(Resources.LoadAll<MechaDesc>("")[0]);
        }   
    }

}

