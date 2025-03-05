using System;
using System.Collections.Generic;
using System.Linq;
using Mekaiju.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Mekaiju.Entity;
using Mekaiju.Entity.Effect;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InstanceContext
    {
        public InputAction moveAction;

        public MechaAnimatorProxy animationProxy;
        public Rigidbody rigidbody;
    }


    /// <summary>
    /// 
    /// </summary>
    public class MechaInstance : IEntityInstance
    {
        /// <summary>
        /// 
        /// </summary>
        public MechaDesc desc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] 
        private EnumArray<MechaPart, MechaPartInstance> _parts;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
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

#region MechaInstance specifique implementation
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return health > 0;
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
        #endregion

        #region MonoBehaviour implementation
        private void Awake()
        {
            onAddEffect = new();
            onRemoveEffect = new();
        }

        private void Start()
        {
            desc = GameManager.instance.playerData.mechaDesc;

            effects = new()
            {
                new(this, Resources.Load<Effect>("Mecha/Objects/Effect/Stamina")),
                new(this, Resources.Load<Effect>("Mecha/Objects/Effect/Heal")),
            };

            stamina = desc.stamina;

            context = new()
            {
                animationProxy = GetComponent<MechaAnimatorProxy>(),
                rigidbody      = GetComponent<Rigidbody>(),
            };

            var t_main = Instantiate(desc.prefab, transform);
            _parts = desc.parts.Select((key, part) => 
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
#endregion

#region IEntityInstance implementation
        public override float baseHealth => desc.parts.Aggregate(0f, (t_acc, t_part) => t_acc + t_part.health);

        public override bool isAlive => health > 0;

        public override void Heal(float p_amount)
        {
            foreach (var t_part in _parts)
            {
                t_part.Heal(p_amount);
            }
        }

        public override void TakeDamage(float p_damage)
        {
            foreach (var t_part in _parts)
            {
                // TODO: Maybe not divide
                t_part.TakeDamage(p_damage / _parts.Count());    
            }
        }

        public override float baseStamina => desc.stamina;

        public override void RestoreStamina(float p_amount)
        {
            stamina = Math.Min(desc.stamina, stamina + p_amount);
        }

        public override void ConsumeStamina(float p_amount)
        {
            stamina = Math.Max(0, stamina - p_amount);
        }
#endregion
    }
}
