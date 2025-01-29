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
        public MechaConfig Config;

        public PlayerData()
        {
            Config = new(Resources.LoadAll<Mecha>("")[0]);
        }   
    }

}

