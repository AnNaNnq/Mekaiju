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
        /// Track if resource is no more managed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        public EffectState State { get; private set; }

        public StatefullEffect(MechaInstance p_target, Effect p_effect, float p_time)
        {
            State = EffectState.Inactive;

            _target  = p_target; 
            _effect  = p_effect;
            _time    = p_time;
            _elapsed = 0f;

            _effect.Behaviour?.OnAdd(p_target);
        }

        public StatefullEffect(MechaInstance p_target, Effect p_effect) : this(p_target, p_effect, -1)
        {        

        }

        public void Tick()
        {
            if (State != EffectState.Expired)
            {
                if (_time > 0 && _elapsed > _time)
                {
                    State = EffectState.Expired;
                    _effect.Behaviour?.OnRemove(_target);
                }
                else
                {
                    State = EffectState.Active;

                    _effect.Behaviour.Tick(_target);
                    _elapsed += Time.deltaTime;
                }
            }
        }
        
        public void FixedTick()
        {
            if (_time < 0 || _elapsed < _time)
            {
                _effect.Behaviour.FixedTick(_target);
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
                _effect.Behaviour?.OnRemove(_target);
            }

            _disposed = true;
        }
    }
}