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
    public class StatefullEffect : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private MechaInstance _target;

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
        /// Track if resource is no longer managed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        public EffectState state { get; private set; }

        public StatefullEffect(MechaInstance p_target, Effect p_effect, float p_time)
        {
            state = EffectState.Inactive;

            _target  = p_target; 
            _effect  = p_effect;
            _time    = p_time;
            _elapsed = 0f;

            _effect.behaviour?.OnAdd(p_target);
        }

        public StatefullEffect(MechaInstance p_target, Effect p_effect) : this(p_target, p_effect, -1)
        {        

        }

        public void Tick()
        {
            if (state != EffectState.Expired)
            {
                if (_time > 0 && _elapsed > _time)
                {
                    state = EffectState.Expired;
                    _effect.behaviour?.OnRemove(_target);
                }
                else
                {
                    state = EffectState.Active;

                    _effect.behaviour.Tick(_target);
                    _elapsed += Time.deltaTime;
                }
            }
        }
        
        public void FixedTick()
        {
            if (_time < 0 || _elapsed < _time)
            {
                _effect.behaviour.FixedTick(_target);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool p_disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (p_disposing)
            {
                _effect.behaviour?.OnRemove(_target);
            }

            _disposed = true;
        }
    }
}