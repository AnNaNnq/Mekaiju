using System;
using System.Collections.Generic;
using System.Linq;
using Mekaiju.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InstanceContext
    {
        public float lastAbilityTime = -1000f;
        public float lastDamageTime  = -1000f;

        public EnumArray<ModifierTarget, ModifierCollection> modifiers = new(() => new());

        public bool isGrounded          = false;
        public bool isMovementAltered   = false;
        public bool isMovementOverrided = false;

        public InputAction moveAction;

        public MechaAnimatorProxy animationProxy;
        public Rigidbody rigidbody;
    }


    /// <summary>
    /// 
    /// </summary>
    public class MechaInstance : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public MechaConfig config { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] 
        private EnumArray<MechaPart, MechaPartInstance> _parts;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        public List<StatefullEffect> effects { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public float health 
        { 
            get
            {
                return _parts.Aggregate(0f, (t_acc, t_part) => { return t_acc + t_part.health; });
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public float stamina { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public InstanceContext context { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public UnityEvent<StatefullEffect> onAddEffect;

        /// <summary>
        /// 
        /// </summary>
        public UnityEvent<StatefullEffect> onRemoveEffect;

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
            config = GameManager.instance.playerData.mechaConfig;

            effects = new()
            {
                new(this, Resources.Load<Effect>("Mecha/Objects/Effect/Stamina")),
                new(this, Resources.Load<Effect>("Mecha/Objects/Effect/Heal")),
            };

            stamina = config.desc.stamina;

            context = new()
            {
                animationProxy = GetComponent<MechaAnimatorProxy>(),
                rigidbody      = GetComponent<Rigidbody>(),
            };

            var t_main = Instantiate(config.desc.prefab, transform);
            _parts = config.parts.Select((key, part) => 
                {
                    Transform  t_tr;
                    GameObject t_go;
                    MechaPartInstance t_inst;

                    t_tr = t_main.transform.FindNested(Enum.GetName(typeof(MechaPart), key) + "Anchor");
                    Debug.Assert(t_tr, $"Unable to find an anchor for {Enum.GetName(typeof(MechaPart), key)}!");
                    t_tr.gameObject.SetActive(false);

                    t_go = t_tr.Find(part.ability.objectName).gameObject;
                    Debug.Assert(t_go, $"Unable to find the GameObject associated to the ability {part.ability.name}!");

                    t_inst = t_go.AddComponent<MechaPartInstance>();
                    t_inst.Initialize(this, part);

                    t_tr.gameObject.SetActive(true);

                    return t_inst;
                }
            );
                        
            onAddEffect = new();
            onRemoveEffect = new();
        }

        private void Update()
        {            
            effects.ForEach  (effect => effect.Tick());
            effects.RemoveAll(effect => 
            {
                if (effect.state == EffectState.Expired)
                {
                    onRemoveEffect.Invoke(effect);
                    effect.Dispose();
                    return true;
                }
                return false;
            });
        }

        private void FixedUpdate()
        {
            effects.ForEach(effect => effect.FixedTick());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return health > 0;
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
            effects.Add(new(this, p_effect));
            onAddEffect.Invoke(effects[^1]);
            return effects[^1];
        }

        /// <summary>
        /// Adds a new effect to the list of active effects, with a specified duration.
        /// </summary>
        /// <param name="p_effect">The effect to be added.</param>
        /// <param name="p_time">The duration of the effect in seconds.</param>
        public IDisposable AddEffect(Effect p_effect, float p_time)
        {
            effects.Add(new(this, p_effect, p_time));
            onAddEffect.Invoke(effects[^1]);
            return effects[^1];
        }

        /// <summary>
        /// Remove an effect.
        /// </summary>
        /// <param name="p_effect">The effect to remove.</param>
        public void RemoveEffect(IDisposable p_effect)
        {
            if (typeof(StatefullEffect).IsAssignableFrom(p_effect.GetType()))
            {
                onRemoveEffect.Invoke((StatefullEffect)p_effect);
                effects.Remove((StatefullEffect)p_effect);
                p_effect.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_amount"></param>
        public void RestoreStamina(float p_amount)
        {
            stamina = Math.Min(config.desc.stamina, stamina + p_amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_amount"></param>
        public void ConsumeStamina(float p_amount)
        {
            stamina = Math.Max(0, stamina - p_amount);
        }
    }

}
