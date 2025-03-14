using System;
using UnityEngine;
using System.Linq;

namespace Mekaiju.Entity.Effect
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
        private EntityInstance _target;

        /// <summary>
        /// 
        /// </summary>
        public Effect effect { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public float time { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private float _elapsed;

        /// <summary>
        /// 
        /// </summary>
        public float remainingTime => time > 0 ? time - _elapsed : -1;

        /// <summary>
        /// Track if resource is no longer managed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        public EffectState state { get; private set; }

        public StatefullEffect(EntityInstance p_target, Effect p_effect, float p_time)
        {
            state = EffectState.Inactive;

            _target  = p_target; 
            effect   = ScriptableObject.Instantiate(p_effect);
            time     = p_time;
            _elapsed = 0f;

            effect.behaviour?.OnAdd(p_target);
        }

        public StatefullEffect(EntityInstance p_target, Effect p_effect) : this(p_target, p_effect, -1)
        {        

        }

        public void Tick()
        {
            if (state != EffectState.Expired)
            {
                if (time > 0 && _elapsed > time)
                {
                    state = EffectState.Expired;
                    effect.behaviour?.OnRemove(_target);
                }
                else
                {
                    state = EffectState.Active;

                    effect.behaviour.Tick(_target);
                    _elapsed += Time.deltaTime;
                }
            }
        }
        
        public void FixedTick()
        {
            if (time < 0 || _elapsed < time)
            {
                effect.behaviour.FixedTick(_target);
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
                effect.behaviour?.OnRemove(_target);
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Class extension for effects
    /// </summary>
    public static class EffectListExtension
    {
        public static string ToString<T>(this System.Collections.Generic.List<T> p_effects, string[] p_exclude) where T : StatefullEffect
        {
            var filteredEffects = p_effects
                .Select(effect => effect.effect.description)
                .Where(desc => !p_exclude.Contains(desc))
                .ToList();

            return filteredEffects.Count > 0 ? string.Join(" & ", filteredEffects) : string.Empty;
        }
    }

}