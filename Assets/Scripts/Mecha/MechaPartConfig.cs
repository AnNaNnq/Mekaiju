using UnityEngine;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    public class MechaPartConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public MechaPartDesc desc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private Ability _ability;
        public  Ability ability 
        { 
            get => _ability;
            set
            {
                if (desc.isAbilityReplaceable)
                {
                    _ability = value;
                }
                else
                {
                    Debug.LogWarning("You can't override ability on this part!");
                }
            }    
        }

        public MechaPartConfig(MechaPartDesc p_desc)
        {
            desc     = p_desc;
            _ability = p_desc.Ability;
        }
    }
}
