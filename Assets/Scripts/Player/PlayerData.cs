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
        public MechaDesc mechaDesc;

        public PlayerData()
        {
            mechaDesc = ScriptableObject.Instantiate(Resources.LoadAll<MechaDesc>("")[0]);
        }   
    }

}

