using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju
{
    public class MechaAnimatorProxy : MonoBehaviour
    {
        public Animator animator;

        public UnityEvent onJump;
        public UnityEvent onFire;

        void Awake()
        {
            animator = GetComponent<Animator>();
            onJump   = new();
            onFire   = new();
        }

        private void _OnJump()
        {
            onJump.Invoke();
        }

        private void _OnFire()
        {
            onFire.Invoke();
        }
    }
}
