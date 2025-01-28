using System;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MechaPartInstance : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        private MechaPartConfig _config;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _health;

        /// <summary>
        /// 
        /// </summary>
        public Ability Ability { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private bool _isDefautlAbility;

        // public MechaPartInstance(MechaPartConfig config)
        // {
        //     _config  = config;
        //     _health = config.Base.Health;
        //     Ability = config.Base.DefaultAbility;
        //     _isDefautlAbility = true;
        // }

        public void Initialize(MechaPartConfig p_config)
        {
            _config  = p_config;
            _health = p_config.Base.Health;
            Ability = p_config.Base.DefaultAbility;
            _isDefautlAbility = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SwapAbility()
        {
            if (_isDefautlAbility)
            {
                Ability = _config.Special;
                _isDefautlAbility = false;
            }
            else
            {
                Ability = _config.Base.DefaultAbility;
                _isDefautlAbility = true;
            }
        }
    }

}
