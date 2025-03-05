using UnityEngine;

namespace Mekaiju.AI
{
    [RequireComponent(typeof(Animator))]
    public class KaijuAnimatorController : MonoBehaviour
    {
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void AttackAnimation(string p_animName)
        {
            _animator.SetTrigger(p_animName);
        }
    }
}