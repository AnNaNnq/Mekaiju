using System.Reflection;
using UnityEngine;

namespace Mekaiju
{
    public class MechaAnimationEnterHandler : StateMachineBehaviour
    {
        public string target;

        private readonly BindingFlags _flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.gameObject.TryGetComponent<MechaAnimatorProxy>(out var t_proxy))
            {
                t_proxy.GetType().InvokeMember($"_On{target}Start", _flags, null, t_proxy, null);
            }
            else
            {
                Debug.LogWarning("Missing MechaAnimatorProxy on animator owning gameobject! This could lead to UB!");
            }
        }
    }
}
