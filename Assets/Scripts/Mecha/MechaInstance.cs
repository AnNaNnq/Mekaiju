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
        public float LastDamageTime  = -1000f;

        public EnumArray<ModifierTarget, ModifierCollection> Modifiers = new(() => new());

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
                    Transform  t_tr;
                    GameObject t_go;
                    MechaPartInstance t_inst;

                    t_tr = t_main.transform.FindNested(Enum.GetName(typeof(MechaPart), key) + "Anchor");
                    Debug.Assert(t_tr, $"Unable to find an anchor for {Enum.GetName(typeof(MechaPart), key)}!");
                    t_tr.gameObject.SetActive(false);

                    t_go = t_tr.Find(part.Ability.ObjectName).gameObject;
                    Debug.Assert(t_go, $"Unable to find the GameObject associated to the ability {part.Ability.name}!");

                    t_inst = t_go.AddComponent<MechaPartInstance>();
                    t_inst.Initialize(this, part);

                    t_tr.gameObject.SetActive(true);

                    return t_inst;
                }
            );

            _effects = new()
            {
                new(this, Resources.Load<Effect>("Mecha/Objects/Effect/Stamina")),
                new(this, Resources.Load<Effect>("Mecha/Objects/Effect/Heal")),
            };

            Stamina = Desc.Stamina;

            Context = new()
            {
                Animator  = GetComponent<Animator>(),
                Rigidbody = GetComponent<Rigidbody>(),
            };
        }

        private void Update()
        {            
            _effects.ForEach  (effect => effect.Tick());
            _effects.RemoveAll(effect => 
            {
                if (effect.State == EffectState.Expired)
                {
                    effect.Dispose();
                    return true;
                }
                return false;
            });
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
        public IDisposable AddEffect(Effect p_effect)
        {
            _effects.Add(new(this, p_effect));
            return _effects[^1];
        }

        /// <summary>
        /// Adds a new effect to the list of active effects, with a specified duration.
        /// </summary>
        /// <param name="p_effect">The effect to be added.</param>
        /// <param name="p_time">The duration of the effect in seconds.</param>
        public IDisposable AddEffect(Effect p_effect, float p_time)
        {
            _effects.Add(new(this, p_effect, p_time));
            return _effects[^1];
        }

        /// <summary>
        /// Remove an effect.
        /// </summary>
        /// <param name="p_effect">The effect to remove.</param>
        public void RemoveEffect(IDisposable p_effect)
        {
            if (typeof(StatefullEffect).IsAssignableFrom(p_effect.GetType()))
            {
                _effects.Remove((StatefullEffect)p_effect);
                p_effect.Dispose();
            }
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
