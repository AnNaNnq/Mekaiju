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
        private MechaPartDesc _desc;

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

        public void Initialize(MechaPartDesc p_config)
        {
            _desc   = p_config;
            _health = p_config.Health;
            Ability = p_config.DefaultAbility;
            _isDefautlAbility = true;

            _desc.DefaultAbility.Behaviour?.Initialize();
            _desc.SpecialAbility?.Behaviour?.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SwapAbility()
        {
            if (_isDefautlAbility && _desc.HasSpecial)
            {
                Ability = _desc.SpecialAbility;
                _isDefautlAbility = false;
            }
            else
            {
                Ability = _desc.DefaultAbility;
                _isDefautlAbility = true;
            }
        }
    }

}
