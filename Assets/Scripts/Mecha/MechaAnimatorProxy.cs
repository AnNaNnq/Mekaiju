using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju
{   
    public enum AnimationState
    {
        Start, End, Trigger, Idle
    }

    public class AnimationEvent
    {
        public AnimationState state;
        public object         payload;

        public AnimationEvent(AnimationState p_state)
        {
            state   = p_state;
            payload = null;
        }

        public AnimationEvent(AnimationState p_state, object p_payload)
        {
            state   = p_state;
            payload = p_payload;
        }
    }

    public class MechaAnimatorProxy : MonoBehaviour
    {
        public Animator animator;

        public UnityEvent<AnimationEvent> onJump;
        public UnityEvent<AnimationEvent> onDash;
        public UnityEvent<AnimationEvent> onLArm;
        public UnityEvent<AnimationEvent> onRArm;

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            onJump   = new();
            onLArm   = new();
            onRArm   = new();
            onDash   = new();
        }

#region Jump
        private void _OnJumpStart()
        {
            onJump.Invoke(new(AnimationState.Start));
        }

        private void _OnJumpTrigger(int p_number)
        {
            onJump.Invoke(new(AnimationState.Trigger, p_number));
        }

        private void _OnJumpEnd()
        {
            onJump.Invoke(new(AnimationState.End));
        }
#endregion

#region LArm
        private void _OnLArmStart()
        {
            onLArm.Invoke(new(AnimationState.Start));
        }

        private void _OnLArmTrigger(int p_number)
        {
            onLArm.Invoke(new(AnimationState.Trigger, p_number));
        }

        private void _OnLArmEnd()
        {
            onLArm.Invoke(new(AnimationState.End));
        }
#endregion

#region RArm
        private void _OnRArmStart()
        {
            onRArm.Invoke(new(AnimationState.Start));
        }

        private void _OnRArmTrigger(int p_number)
        {
            onRArm.Invoke(new(AnimationState.Trigger, p_number));
        }

        private void _OnRArmEnd()
        {
            onRArm.Invoke(new(AnimationState.End));
        }
        #endregion

#region Dash
        private void _OnDashStart()
        {
            onDash.Invoke(new(AnimationState.Start));
        }

        private void _OnDashTrigger(int p_number)
        {
            onDash.Invoke(new(AnimationState.Trigger, p_number));
        }

        private void _OnDashEnd()
        {
            onDash.Invoke(new(AnimationState.End));
        }
        #endregion
    }
}
