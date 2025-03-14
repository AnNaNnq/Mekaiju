using System;
using System.Linq;
using Mekaiju.Utils;
using UnityEngine;
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
        public MechaAnimatorProxy animationProxy;
        public Rigidbody rigidbody;
    }


    /// <summary>
    /// 
    /// </summary>
    public class MechaInstance : EntityInstance
    {
        /// <summary>
        /// 
        /// </summary>
        public MechaDesc desc { get; private set; }

        /// <summary>
        /// The current stamina of this entity.
        /// </summary>
        private float _stamina;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] 
        private EnumArray<MechaPart, MechaPartInstance> _parts;

        public Ability shieldAbility;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_part"></param>
        /// <returns></returns>
        public MechaPartInstance this[MechaPart p_part]
        {
            get => _parts[p_part];
        }

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
                    t_inst.Initialize(this, part);

                    t_tr.gameObject.SetActive(true);

                    return t_inst;
                }
            );
        }

        public override void Update()
        {
            base.Update();
            shieldAbility.behaviour.Tick(this);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            shieldAbility.behaviour.FixedTick(this);
        }
        #endregion

        #region IEntityInstance implementation
        protected override EnumArray<Statistics, float> statistics => desc.statistics;

        public override bool isAlive => health > 0;

        public override float health     => _parts.Aggregate(0f, (t_acc, t_part) => { return t_acc + t_part.health; });
        public override float baseHealth => desc.statistics[Statistics.Health];

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
}
