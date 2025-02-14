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

            t_go = GameObject.Instantiate(_vfxDefaultPrefab, p_self.Mecha.transform);
            _vfxDefault = t_go.GetComponent<VisualEffect>();
            _vfxDefault.enabled = false;

            t_go = GameObject.Instantiate(_vfxBreakPrefab, p_self.Mecha.transform);
            _vfxBreak = t_go.GetComponent<VisualEffect>();
            _vfxBreak.enabled = false;

            _isActive = false;
            _isStopRequested = false;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (!_isActive)
            {
                _isActive = true;
                _vfxDefault.enabled = true;

                p_self.Mecha.Context.Animator.SetBool("IsShielding", true);
                
                // TODO: rework if other modifier
                var t_sMod = p_self.Mecha.Context.Modifiers[ModifierTarget.Speed]  .Add(_speedModifier);
                var t_dMod = p_self.Mecha.Context.Modifiers[ModifierTarget.Defense].Add(_defenseModifier);
                // p_self.Mecha.Context.SpeedModifier   = _speedModifier;
                // p_self.Mecha.Context.DefenseModifier = _defenseModifier;

                p_self.Mecha.Context.IsMovementAltered = true;

                while (!_isStopRequested && p_self.Mecha.CanExecuteAbility(_consumption * Time.deltaTime))
                {
                    p_self.Mecha.ConsumeStamina(_consumption * Time.deltaTime);
                    p_self.Mecha.Context.LastAbilityTime = Time.time;
                    yield return null;
                }

                _vfxDefault.enabled = false;

                p_self.Mecha.Context.Animator.SetBool("IsShielding", false);
                // TODO: rework if other modifier
                // p_self.Mecha.Context.SpeedModifier   = 1f;
                // p_self.Mecha.Context.DefenseModifier = 1f;
                p_self.Mecha.Context.Modifiers[ModifierTarget.Speed]  .Remove(t_sMod);
                p_self.Mecha.Context.Modifiers[ModifierTarget.Defense].Remove(t_dMod);

                p_self.Mecha.Context.IsMovementAltered = false;
                
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

        public override float Consumption(object p_opt)
        {
            return _consumption;
        }
    }
}
