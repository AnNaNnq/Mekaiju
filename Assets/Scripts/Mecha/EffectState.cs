using System;
using JetBrains.Annotations;
using Mekaiju;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[Serializable]
public class EffectState
{
    /// <summary>
    /// 
    /// </summary>
    private Effect _effect;
    
    /// <summary>
    /// 
    /// </summary>
    private float _elapsed;

    /// <summary>
    /// 
    /// </summary>
    public bool ToRemove => _effect.EffectiveTime > 0 && _elapsed > _effect.EffectiveTime;

    public EffectState(Effect p_effect)
    {
        _effect  = p_effect;
        _elapsed = 0f;
    }

    public void Tick(MechaInstance p_self)
    {
        _effect.Behaviour.Tick(p_self);
        _elapsed += Time.deltaTime;
    }
}
