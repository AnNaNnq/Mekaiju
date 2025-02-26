using UnityEngine;

namespace Mekaiju.AI
{
    [RequireComponent(typeof(Animator))]
    public class KaijuAnimatorController : MonoBehaviour
    {
        private Animator _animator;
        private KaijuInstance _instance;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _instance = GetComponent<KaijuInstance>();
        }

        public void AttackAnimation(string p_animName)
        {
            _animator.SetTrigger(p_animName);
        }
    }
}