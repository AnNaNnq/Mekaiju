using System;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        /// <summary>
        /// 
        /// </summary>
        public MechaDesc mechaDesc;

        public void OnAwake()
        {
            mechaDesc = ScriptableObject.Instantiate(Resources.LoadAll<MechaDesc>("")[0]);
        }
    }

}

