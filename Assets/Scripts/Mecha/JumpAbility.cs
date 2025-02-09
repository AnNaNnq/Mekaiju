using System.Collections;
using Mekaiju;
using Mekaiju.AI;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class JumpAbility : IAbilityBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private float _force;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private int _consumption;

    /// <summary>
    /// 
    /// </summary>
    private bool _requested;

    public override void Initialize()
    {
        _requested = false;
    }

    public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
    {  
        if (p_self.Mecha.Context.IsGrounded && !_requested)
        {
            p_self.Mecha.ConsumeStamina(_consumption);
            _requested = true;
        }
        yield return null;
    }

    public override void FixedTick(MechaPartInstance p_self)
    {
        if (_requested)
        {
            p_self.Mecha.Context.Animator.SetTrigger("Jump");
            p_self.Mecha.Context.Rigidbody.AddForce(Vector3.up * _force, ForceMode.Impulse);
            _requested = false;
        }
    }

    public override float Consumption(object p_opt)
    {
        return _consumption;
    }
}
