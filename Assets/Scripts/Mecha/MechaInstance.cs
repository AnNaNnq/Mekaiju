using System;
using System.Linq;
using Mekaiju.Utils;
using UnityEngine;
using Mekaiju.Entity;
using Mekaiju.Entity.Effect;

namespace Mekaiju
{
    using HealthStatisticValue = Mekaiju.Utils.EnumArray<Mekaiju.MechaPart, float>;

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
        /// 
        /// </summary>
        private float _health;

        /// <summary>
        /// The current stamina of this entity.
        /// </summary>
        private float _stamina;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] 
        private EnumArray<MechaPart, MechaPartInstance> _parts;

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
            _health  = baseHealth; 

            desc.standalones.ForEach((_, t_ability) => t_ability.behaviour.Initialize(this));

            var t_main = GameObject.Find("Aegis");
            _parts = desc.parts.Select((key, part) => 
                {
                    Transform  t_tr;
                    GameObject t_go;
                    MechaPartInstance t_inst;

                    t_tr = t_main.transform;
                    t_tr.gameObject.SetActive(false);

                    t_go = t_tr.FindNested(part.ability.objectName).gameObject;

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
            desc.standalones.ForEach((_, t_ability) => t_ability.behaviour.Tick(this));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            desc.standalones.ForEach((_, t_ability) => t_ability.behaviour.FixedTick(this));
        }
#endregion

        /// <summary>
        /// Should be called only by MechaPartInstance.
        /// </summary>
        /// <param name="p_damage">The amount of damage to deal.</param>
        public void TakeDamage(float p_damage)
        {
            _health = Mathf.Max(0f, _health - p_damage);
        }

        /// <summary>
        /// Should be called only by MechaPartInstance.
        /// </summary>
        /// <param name="p_amount">The amount of health to restore.</param>
        public void HHeal(float p_amount)
        {
            _health = Mathf.Min(baseHealth, _health + p_amount);
        }

#region IEntityInstance implementation
        public override EnumArray<StatisticKind, IStatistic> statistics => desc.statistics;

        public override bool isAlive => health > 0;

        public override float health     => _health;
        public override float baseHealth => statistics[StatisticKind.Health].Apply<HealthStatisticValue>(modifiers[StatisticKind.Health]).Sum();

        public override float Heal(float p_amount)
        {
            var t_dist = p_amount / _parts.Count();
            var t_heal = _parts.Aggregate(0f, (t_acc, t_part) => t_acc + t_part.HHeal(t_dist));

            HHeal(t_heal);
            return t_heal;
        }

        public override float TakeDamage(IDamageable p_from, float p_damage, DamageKind p_kind)
        {
            onBeforeTakeDamage.Invoke(p_from, p_damage, p_kind);
            if (!states[StateKind.Invulnerable])
            {
                timePoints[TimePoint.LastDamage] = Time.time;

                var t_defense = statistics[StatisticKind.Defense].Apply<float>(modifiers[StatisticKind.Defense]);
                var t_damage  = p_damage - p_damage * t_defense;
                var t_dist    = t_damage / _parts.Count();
                var t_taken   = _parts.Aggregate(0f, (t_acc, t_part) => t_acc + t_part.TakeDamage(t_dist));

                TakeDamage(t_taken);
                onAfterTakeDamage.Invoke(p_from, t_taken, p_kind);
                Debug.Log($"base : {p_damage}, after_defense: {t_damage}, t_taken: {t_taken}");
                return t_taken;
            }

            return 0f;
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
