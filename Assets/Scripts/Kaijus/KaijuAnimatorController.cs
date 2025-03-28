using Mekaiju.Entity;
using Mekaiju.Tuto;
using UnityEngine;

namespace Mekaiju.AI
{
    [RequireComponent(typeof(Animator))]
    public class KaijuAnimatorController : MonoBehaviour
    {
        private Animator _animator;
        private EntityInstance _instance;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _instance = GetComponent<KaijuInstance>();
        }

        private void Update()
        {
            if (_animator == null || !HasParameter("speed")) return;

            if (_instance as KaijuInstance)
            {
                KaijuInstance t_kaiju = (KaijuInstance)_instance;
                _animator.SetFloat("speed", t_kaiju.motor.IsInMovement() ? 1 : 0);
            }
            else if (_instance as TutorialInstance)
            {
                TutorialInstance t_tuto = (TutorialInstance)_instance;
                _animator.SetFloat("speed", t_tuto.IsInMovement() ? 1 : 0);
            }
        }

        public void AttackAnimation(string p_animName)
        {
            if(_animator != null && HasParameter(p_animName)) _animator.SetTrigger(p_animName);
        }

        bool HasParameter(string p_paramName)
        {
            if (_animator == null) return false;

            foreach (AnimatorControllerParameter param in _animator.parameters)
            {
                if (param.name == p_paramName)
                    return true;
            }
            return false;
        }
    }
}