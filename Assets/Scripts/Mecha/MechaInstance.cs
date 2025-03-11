using System;
using System.Collections.Generic;
using System.Linq;
using Mekaiju.Utils;
using UnityEngine;
<<<<<<< HEAD
=======
using UnityEngine.Events;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
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
<<<<<<< HEAD
        public float LastAbilityTime = -1000f;

        // public float SpeedModifier   = 1f;
        // public float DefenseModifier = 1f;

        public EnumArray<ModifierTarget, ModifierCollection> Modifiers = new(() => new());

        public bool IsGrounded          = false;
        public bool IsMovementAltered   = false;
        public bool IsMovementOverrided = false;

        public InputAction MoveAction;

        public Animator  Animator;
        public Rigidbody Rigidbody;
=======
        public MechaAnimatorProxy animationProxy;
        public Rigidbody rigidbody;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
    }


    /// <summary>
    /// 
    /// </summary>
<<<<<<< HEAD
    public class MechaInstance : MonoBehaviour
=======
    public class MechaInstance : EntityInstance
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
    {
        /// <summary>
        /// 
        /// </summary>
<<<<<<< HEAD
        public MechaConfig config { get; private set; }
=======
        public MechaDesc desc { get; private set; }

        /// <summary>
        /// The current stamina of this entity.
        /// </summary>
        private float _stamina;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] 
        private EnumArray<MechaPart, MechaPartInstance> _parts;

<<<<<<< HEAD
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
        [field: SerializeField]
        public float Stamina { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public InstanceContext Context { get; private set; }
=======
        public Ability shieldAbility;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_part"></param>
        /// <returns></returns>
        public MechaPartInstance this[MechaPart p_part]
        {
            get => _parts[p_part];
        }

<<<<<<< HEAD
        private void Start()
        {
            Debug.Assert(GameManager.Instance, "Missing GameManager. Please add one!");
            config = GameManager.Instance.playerData.mechaConfig;

            var t_main = Instantiate(config.desc.Prefab, transform);

            _parts = config.parts.Select((key, part) => 
                {
                    var t_tr = t_main.transform.FindNested(Enum.GetName(typeof(MechaPart), key) + "Anchor");
                    Debug.Assert(t_tr);

                    t_tr.gameObject.SetActive(false);

<<<<<<< HEAD
                    Debug.Assert(part.Ability.Prefab);
                    var t_go = Instantiate(part.Ability.Prefab, t_tr);
=======
                    t_go = t_tr.Find(part.ability.ObjectName).gameObject;
                    Debug.Assert(t_go, $"Unable to find the GameObject associated to the ability {part.ability.name}!");
>>>>>>> f6493d6 (feat: allow capacity change)

                    var t_inst = t_go.AddComponent<MechaPartInstance>();
=======
#region MonoBehaviour implementation
        private void Start()
        {
            desc = GameManager.instance.playerData.mechaDesc;

            AddEffect(Resources.Load<Effect>("Mecha/Objects/Effect/Stamina"));
            AddEffect(Resources.Load<Effect>("Mecha/Objects/Effect/Heal"));

            _stamina = desc.stamina;

            shieldAbility = Resources.Load<Ability>("Mecha/Objects/Ability/ShieldAbility");
            shieldAbility.behaviour.Initialize(this);

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
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
                    t_inst.Initialize(this, part);

                    t_tr.gameObject.SetActive(true);

                    return t_inst;
                }
            );
<<<<<<< HEAD

            // TODO: remove
            // t_main.SetActive(false);

            // _effects = new();
            _effects = new()
            {
                new(Resources.Load<Effect>("Mecha/Effect/Stamina")),
                new(Resources.Load<Effect>("Mecha/Effect/Health")),
            };

            Stamina = config.desc.Stamina;

            Context = new()
            {
                Animator  = GetComponent<Animator>(),
                Rigidbody = GetComponent<Rigidbody>(),
                // Modifiers  = new()
            };
        }

        private void Update()
        {            
            _effects.ForEach  (effect => effect.Tick(this));
            _effects.RemoveAll(effect => effect.State == EffectState.Expired);
        }

        private void FixedUpdate()
        {
            _effects.ForEach(effect => effect.FixedTick(this));
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
=======
        }
#endregion

#region IEntityInstance implementation
        protected override EnumArray<Statistics, float> statistics => desc.statistics;

        public override bool isAlive => health > 0;

        public override float health     => _parts.Aggregate(0f, (t_acc, t_part) => { return t_acc + t_part.health; });
        public override float baseHealth => desc.statistics[Statistics.Health];

        public override void Heal(float p_amount)
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
        {
            foreach (var t_part in _parts)
            {
                t_part.Heal(p_amount);
            }
        }

<<<<<<< HEAD
        /// <summary>
        /// Get all the effects that are affecting the mecha
        /// </summary>
        public string GetEffects()
        {
            var filteredEffects = _effects
                .Select(effect => effect.effect.description)
                .Where(desc => desc != "Heal" && desc != "Stamina")
                .ToList();

            return filteredEffects.Count > 0 ? string.Join(" & ", filteredEffects) : string.Empty;
        }

        /// <summary>
        /// Adds a new effect to the list of active effects without a timeout. 
        /// The effect will remain active indefinitely until it is manually removed.
        /// </summary>
        /// <param name="p_effect">The effect to be added.</param>
        public StatefullEffect AddEffect(Effect p_effect)
        {
            _effects.Add(new(p_effect));
            return _effects[^1];
        }

        /// <summary>
        /// Adds a new effect to the list of active effects, with a specified duration.
        /// </summary>
        /// <param name="p_effect">The effect to be added.</param>
        /// <param name="p_time">The duration of the effect in seconds.</param>
        public StatefullEffect AddEffect(Effect p_effect, float p_time)
        {
            _effects.Add(new(p_effect, p_time));
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
            Stamina = Math.Min(config.desc.Stamina, Stamina + p_amount);
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

=======
        public override void TakeDamage(float p_damage)
        {
            foreach (var t_part in _parts)
            {
                t_part.TakeDamage(p_damage / _parts.Count());    
            }
        }

        public override float stamina     => _stamina;
        public override float baseStamina => desc.stamina;

        public override void RestoreStamina(float p_amount)
        {
            _stamina = Math.Min(baseStamina, _stamina + p_amount);
        }

        public override void ConsumeStamina(float p_amount)
        {
            _stamina = Math.Max(0, _stamina - p_amount);
        }
#endregion
    }
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
}
