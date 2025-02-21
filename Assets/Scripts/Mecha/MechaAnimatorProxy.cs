using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju
{
    public enum AnimationEventType
    {
        Start, End, Action
    }

    public class MechaAnimatorProxy : MonoBehaviour
    {
        public Animator animator;

        public UnityEvent<AnimationEventType> onJump;
        public UnityEvent<AnimationEventType> onLArm;
        public UnityEvent<AnimationEventType> onRArm;

        void Awake()
        {
            animator = GetComponent<Animator>();
            onJump   = new();
            onLArm   = new();
            onRArm   = new();
        }

#region Jump
        private void _OnJumpStart()
        {
            onJump.Invoke(AnimationEventType.Start);
        }

        private void _OnJumpAction()
        {
            onJump.Invoke(AnimationEventType.Action);
        }

        private void _OnJumpEnd()
        {
            onJump.Invoke(AnimationEventType.End);
        }
#endregion

#region LArm
        private void _OnLArmStart()
        {
            onLArm.Invoke(AnimationEventType.Start);
        }

        private void _OnLArmAction()
        {
            onLArm.Invoke(AnimationEventType.Action);
        }

        private void _OnLArmEnd()
        {
            onLArm.Invoke(AnimationEventType.End);
        }
#endregion

#region RArm
        private void _OnRArmStart()
        {
            onRArm.Invoke(AnimationEventType.Start);
        }

        private void _OnRArmAction()
        {
            onRArm.Invoke(AnimationEventType.Action);
        }

        private void _OnRArmEnd()
        {
            onRArm.Invoke(AnimationEventType.End);
        }
#endregion
    }
}
