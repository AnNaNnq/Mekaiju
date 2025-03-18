using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using UnityEngine;
using UnityEngine.VFX;
using Mekaiju.Entity;
using MyBox;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    public class ShieldAbility : IAbilityBehaviour
    {
#region Parameter
        [Header("General")]

        /// <summary>
        /// Adjusts speed by a percentage. 0% means no change.
        /// </summary>
        [SerializeField]
        [OverrideLabel("Speed Modifier (%)")]
        [Tooltip("Adjusts speed by a percentage. 0% means no change.")]
        private float _speedModifier;

        [Header("Shield energy relative")]

        /// <summary>
        /// The default amount of energy the shield starts with.
        /// </summary>
        [SerializeField]
        [Tooltip("The default amount of energy the shield starts with.")]
        private float _baseEnergy;

        /// <summary>
        /// The rate at which energy is gained per second.
        /// </summary>
        [SerializeField]
        [OverrideLabel("Fill Rate (per s)")]
        [Tooltip("The rate at which energy is gained per second.")]
        private float _fillRate;

        /// <summary>
        /// The rate at which energy is consumed per second.
        /// </summary>
        [SerializeField]
        [OverrideLabel("Drain Rate (per s)")]
        [Tooltip("The rate at which energy is consumed per second.")]
        private float _drainRate;

        [Header("Visual")]

        /// <summary>
        /// The prefab for active shield.
        /// </summary>
        [SerializeField]
        private GameObject _vfxDefaultPrefab;

        /// <summary>
        /// The prefab for breaking shield.
        /// </summary>
        [SerializeField]
        private GameObject _vfxBreakPrefab;
#endregion

        private VisualEffect _vfxDefault;
        private VisualEffect _vfxBreak;

        private float _vfxBreakActiveTime = .25f;

        private float _energy;

        private bool _isBroke;
        private bool _isStopRequested;

        private MechaAnimatorProxy _animationProxy;

        public override void Initialize(EntityInstance p_self)
        {
            base.Initialize(p_self);

            GameObject t_go;

            t_go = GameObject.Instantiate(_vfxDefaultPrefab, p_self.transform.Find("ChestPivot"));
            _vfxDefault = t_go.GetComponent<VisualEffect>();
            _SetVFXState(_vfxDefault, false);

            t_go = GameObject.Instantiate(_vfxBreakPrefab, p_self.transform.Find("ChestPivot"));
            _vfxBreak = t_go.GetComponent<VisualEffect>();
            _SetVFXState(_vfxBreak, false);

            _energy = _baseEnergy;

            _isBroke  = false;
            _isStopRequested = false;

            _animationProxy = p_self.GetComponentInChildren<MechaAnimatorProxy>();

            if (!_animationProxy)
            {
                Debug.LogWarning("Unable to find animator proxy on mecha!");
            }
        }

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return base.IsAvailable(p_self, p_opt) && !_isBroke;
        }

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                state = AbilityState.Active;
                
                var t_sMod = p_self.modifiers[StatisticKind.Speed].Add(_speedModifier / 100f, ModifierKind.Percent);
                p_self.states[State.Protected] = true;

                _SetVFXState(_vfxDefault, true);
                _animationProxy.animator.SetBool("IsShielding", true);
                while (!_isBroke && !_isStopRequested)
                {
                    _energy = Mathf.Max(_energy - _drainRate * Time.deltaTime, 0f);
                    if (_energy <= 0f)
                    {
                        _isBroke = true;
                    }
                    yield return null;
                }
                _SetVFXState(_vfxDefault, false);
                _animationProxy.animator.SetBool("IsShielding", false);

                p_self.modifiers[StatisticKind.Speed].Remove(t_sMod);
                p_self.states[State.Protected] = false;
                
                _SetVFXState(_vfxBreak, true);
                yield return new WaitForSeconds(_vfxBreakActiveTime);
                _SetVFXState(_vfxBreak, false);

                _isStopRequested = false;
                state = AbilityState.Ready;
            }
        }

        public override void Release()
        {
            if (state == AbilityState.Active)
            {
                _isStopRequested = true;
            }
        }

        public override void Tick(EntityInstance p_self)
        {
            if (state != AbilityState.Active)
            {
                _energy = Mathf.Min(_energy + _fillRate * Time.deltaTime, _baseEnergy);
                if (_isBroke && _energy >= _baseEnergy)
                {
                    _isBroke = false;
                }
            }
        }

        private void _SetVFXState(VisualEffect p_effect, bool p_state)
        {
            p_effect.gameObject.SetActive(p_state);
            p_effect.enabled = p_state;
        }
    }
}
