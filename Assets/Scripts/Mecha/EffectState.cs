using System;
using UnityEngine;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    public enum EffectState
    {
        Active, Inactive, Expired
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class StatefullEffect
    {
        /// <summary>
        /// 
        /// </summary>
        private Effect _effect;
        
        /// <summary>
        /// 
        /// </summary>
        private float _time;
        
        /// <summary>
        /// 
        /// </summary>
        private float _elapsed;

        /// <summary>
        /// 
        /// </summary>
        public EffectState State { get; private set; }

        public StatefullEffect(Effect p_effect, float p_time)
        {
            State = EffectState.Inactive;

            _effect  = p_effect;
            _time    = p_time;
            _elapsed = 0f;
        }

        public StatefullEffect(Effect p_effect) : this(p_effect, -1)
        {        

        }

        public void Tick(MechaInstance p_self)
        {
            if (_time > 0 && _elapsed > _time)
            {
                State = EffectState.Expired;
            }
            else
            {
                State = EffectState.Active;

                _effect.Behaviour.Tick(p_self);
                _elapsed += Time.deltaTime;
            }
        }
    }
}