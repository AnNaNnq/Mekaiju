using System;
using System.Collections.Generic;
using System.Linq;
using Mekaiju.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InstanceContext
    {
        public float LastAbilityTime = -1000f;
        public float LastDamageTime = -1000f;

        public float SpeedModifier   = 1f;
        public float DefenseModifier = 1f;

        public bool IsGrounded          = false;
        public bool IsMovementAltered   = false;
        public bool IsMovementOverrided = false;

        public InputAction MoveAction;

        public Animator  Animator;
        public Rigidbody Rigidbody;
    }


    /// <summary>
    /// 
    /// </summary>
    public class MechaInstance : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public MechaDesc Desc;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] 
        private EnumArray<MechaPart, MechaPartInstance> _parts;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<StatefullEffect> _effects;

        /// <summary>
        /// 
        /// </summary>
        public float Health 
        { 
            get
            {
                return _parts.Aggregate(0f, (t_acc, t_part) => { return t_acc + t_part.Health; });
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public float Stamina { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public InstanceContext Context { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_part"></param>
        /// <returns></returns>
        public MechaPartInstance this[MechaPart p_part]
        {
            get => _parts[p_part];
        }

        private void Start()
        {
            // Desc = GameManager.Instance.PData.Mecha;

            Debug.Assert(Desc.Prefab);
            var t_main = Instantiate(Desc.Prefab, transform);

            _parts = Desc.Parts.Select((key, part) => 
                {
                    var t_tr = t_main.transform.FindNested(Enum.GetName(typeof(MechaPart), key) + "Anchor");
                    Debug.Assert(t_tr);

                    t_tr.gameObject.SetActive(false);

                    Debug.Assert(part.DefaultAbility.Prefab);
                    var t_go = Instantiate(part.DefaultAbility.Prefab, t_tr);

                    var t_inst = t_go.AddComponent<MechaPartInstance>();
                    t_inst.Initialize(this, part);

                    t_tr.gameObject.SetActive(true);

                    return t_inst;
                }
            );

            // TODO: remove
            // t_main.SetActive(false);

            // _effects = new();
            _effects = new()
            {
                new(this, Resources.Load<Effect>("Mecha/Objects/Effect/Stamina")),
                new(this, Resources.Load<Effect>("Mecha/Objects/Effect/Heal"), 10),
            };

            Stamina = Desc.Stamina;

            Context = new()
            {
                Animator  = GetComponent<Animator>(),
                Rigidbody = GetComponent<Rigidbody>()
            };
        }

        private void Update()
        {            
            _effects.ForEach  (effect => effect.Tick());
            _effects.RemoveAll(effect => effect.State == EffectState.Expired);
        }

        private void FixedUpdate()
        {
            _effects.ForEach(effect => effect.FixedTick());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return Health > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_damage"></param>
        public void TakeDamage(float p_damage)
        {
            foreach (var t_part in _parts)
            {
                // TODO: Maybe not divide
                t_part.TakeDamage(p_damage / _parts.Count());    
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_amount"></param>
        public void Heal(float p_amount)
        {
            foreach (var t_part in _parts)
            {
                t_part.Heal(p_amount);
            }
        }

        /// <summary>
        /// Adds a new effect to the list of active effects without a timeout. 
        /// The effect will remain active indefinitely until it is manually removed.
        /// </summary>
        /// <param name="p_effect">The effect to be added.</param>
        public StatefullEffect AddEffect(Effect p_effect)
        {
            _effects.Add(new(this, p_effect));
            return _effects[^1];
        }

        /// <summary>
        /// Adds a new effect to the list of active effects, with a specified duration.
        /// </summary>
        /// <param name="p_effect">The effect to be added.</param>
        /// <param name="p_time">The duration of the effect in seconds.</param>
        public StatefullEffect AddEffect(Effect p_effect, float p_time)
        {
            _effects.Add(new(this, p_effect, p_time));
            return _effects[^1];
        }

        public void RemoveEffect(StatefullEffect p_effect)
        {
            _effects.Remove(p_effect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_consumption"></param>
        /// <returns></returns>
        public bool CanExecuteAbility(float p_consumption)
        {
            return Stamina - p_consumption > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_amount"></param>
        public void RestoreStamina(float p_amount)
        {
            Stamina = Math.Min(Desc.Stamina, Stamina + p_amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_amount"></param>
        public void ConsumeStamina(float p_amount)
        {
            Stamina = Math.Max(0, Stamina - p_amount);
        }
    }

}
