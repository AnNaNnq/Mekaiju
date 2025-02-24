using UnityEngine;

namespace Mekaiju.AI
{
    [RequireComponent(typeof(Animator))]
    public class KaijuAnimatorController : MonoBehaviour
    {
        private Animator _animator;
        private KaijuInstance _instance;
        private int animationId;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _instance = GetComponent<KaijuInstance>();
            animationId = 0;
        }

        private void Update()
        {
            CheckAnimation();
        }

        public void CheckAnimation()
        {
            foreach(KaijuAttack attack in _instance.brain.allAttacks)
            {
                _animator.SetBool(attack.name, attack.attack.isUsing);
            }
        }
    }
}