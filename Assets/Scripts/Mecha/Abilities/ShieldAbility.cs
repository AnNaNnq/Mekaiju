using System.Collections;
using Mekaiju.AI;
using UnityEngine;
using UnityEngine.VFX;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    public class ShieldAbility : IAbilityBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _vfxDefaultPrefab;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _vfxBreakPrefab;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _breakTime;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _speedModifier;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _defenseModifier;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _consumption;

        private VisualEffect _vfxDefault;
        private VisualEffect _vfxBreak;

        private bool _isActive;
        private bool _isStopRequested;

        public override void Initialize(MechaPartInstance p_self)
        {
            GameObject t_go;

            t_go = GameObject.Instantiate(_vfxDefaultPrefab, p_self.mecha.transform.Find("ChestPivot"));
            _vfxDefault = t_go.GetComponent<VisualEffect>();
            _vfxDefault.enabled = false;

            t_go = GameObject.Instantiate(_vfxBreakPrefab, p_self.mecha.transform.Find("ChestPivot"));
            _vfxBreak = t_go.GetComponent<VisualEffect>();
            _vfxBreak.enabled = false;

            _isActive = false;
            _isStopRequested = false;
        }

        public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
        {
            return !_isActive && p_self.mecha.stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                _isActive = true;
                _vfxDefault.enabled = true;

                p_self.mecha.context.animationProxy.animator.SetBool("IsShielding", true);
                
                // TODO: rework if other modifier
                var t_sMod = p_self.mecha.context.modifiers[ModifierTarget.Speed]  .Add(_speedModifier);
                var t_dMod = p_self.mecha.context.modifiers[ModifierTarget.Defense].Add(_defenseModifier);

                p_self.mecha.context.isMovementAltered = true;

                while (!_isStopRequested && p_self.mecha.stamina - (_consumption * Time.deltaTime) >= 0)
                {
                    p_self.mecha.ConsumeStamina(_consumption * Time.deltaTime);
                    p_self.mecha.context.lastAbilityTime = Time.time;
                    yield return null;
                }

                _vfxDefault.enabled = false;

                p_self.mecha.context.animationProxy.animator.SetBool("IsShielding", false);
                // TODO: rework if other modifier
                p_self.mecha.context.modifiers[ModifierTarget.Speed]  .Remove(t_sMod);
                p_self.mecha.context.modifiers[ModifierTarget.Defense].Remove(t_dMod);

                p_self.mecha.context.isMovementAltered = false;
                
                _vfxBreak.enabled = true;
                yield return new WaitForSeconds(_breakTime);
                _vfxBreak.enabled = false;

                _isActive = false;
                _isStopRequested = false;
            }
        }

        public override void Release()
        {
            if (_isActive)
            {
                _isStopRequested = true;
            }
        }
    }
}
