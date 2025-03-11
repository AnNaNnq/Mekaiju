using System;
using System.Collections.Generic;
using System.Linq;
using Mekaiju.Utils;
using UnityEngine;
<<<<<<< Updated upstream
<<<<<<< HEAD
=======
using UnityEngine.Events;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
=======
using UnityEngine.Events;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
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
=======
        public MechaAnimatorProxy animationProxy;
        public Rigidbody rigidbody;
>>>>>>> Stashed changes
    }


    /// <summary>
    /// 
    /// </summary>
<<<<<<< Updated upstream
<<<<<<< HEAD
    public class MechaInstance : MonoBehaviour
=======
    public class MechaInstance : EntityInstance
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
=======
    public class MechaInstance : EntityInstance
>>>>>>> Stashed changes
    {
        /// <summary>
        /// 
        /// </summary>
<<<<<<< Updated upstream
<<<<<<< HEAD
        public MechaConfig config { get; private set; }
=======
        public MechaDesc desc { get; private set; }

        /// <summary>
        /// The current stamina of this entity.
        /// </summary>
        private float _stamina;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
=======
        public MechaDesc desc { get; private set; }
>>>>>>> Stashed changes

        /// <summary>
        /// The current stamina of this entity.
        /// </summary>
        private float _stamina;

<<<<<<< HEAD
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private EnumArray<MechaPart, MechaPartInstance> _parts;

<<<<<<< Updated upstream
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
=======
        public Ability shieldAbility;
>>>>>>> Stashed changes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_part"></param>
        /// <returns></returns>
        public MechaPartInstance this[MechaPart p_part]
        {
            get => _parts[p_part];
        }

<<<<<<< Updated upstream
<<<<<<< HEAD
=======
        #region MonoBehaviour implementation
>>>>>>> Stashed changes
        private void Start()
        {
            desc = GameManager.instance.playerData.mechaDesc;

            AddEffect(Resources.Load<Effect>("Mecha/Objects/Effect/Stamina"));
            AddEffect(Resources.Load<Effect>("Mecha/Objects/Effect/Heal"));

            _stamina = desc.stamina;

            shieldAbility = Resources.Load<Ability>("Mecha/Objects/Ability/ShieldAbility");
            shieldAbility.behaviour.Initialize(this);

<<<<<<< Updated upstream
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
=======
            var t_main = Instantiate(desc.prefab, transform);
            _parts = desc.parts.Select((key, part) =>
>>>>>>> Stashed changes
            {
                Transform t_tr;
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
        #endregion

<<<<<<< Updated upstream
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_amount"></param>
        public void Heal(float p_amount)
=======
        }
#endregion

#region IEntityInstance implementation
=======
        #region IEntityInstance implementation
>>>>>>> Stashed changes
        protected override EnumArray<Statistics, float> statistics => desc.statistics;

        public override bool isAlive => health > 0;

<<<<<<< Updated upstream
        public override float health     => _parts.Aggregate(0f, (t_acc, t_part) => { return t_acc + t_part.health; });
        public override float baseHealth => desc.statistics[Statistics.Health];

        public override void Heal(float p_amount)
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
=======
        public override float health => _parts.Aggregate(0f, (t_acc, t_part) => { return t_acc + t_part.health; });
        public override float baseHealth => desc.statistics[Statistics.Health];

        public override void Heal(float p_amount)
>>>>>>> Stashed changes
        {
            foreach (var t_part in _parts)
            {
                t_part.Heal(p_amount);
            }
        }

<<<<<<< Updated upstream
<<<<<<< HEAD
        /// <summary>
        /// Get all the effects that are affecting the mecha
        /// </summary>
        public string GetEffects()
=======
        public override void TakeDamage(float p_damage)
>>>>>>> Stashed changes
        {
            foreach (var t_part in _parts)
            {
                t_part.TakeDamage(p_damage / _parts.Count());
            }
        }

        public override float stamina => _stamina;
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
<<<<<<< Updated upstream

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
=======
}
>>>>>>> Stashed changes
