using System;
using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.Utils;
using UnityEngine;
using UnityEngine.Events;
using Mekaiju.Entity;
using System.Collections.Generic;
using Mekaiju.Entity.Effect;
using System.Linq;

namespace Mekaiju
{
    using HealthStatisticValue = Mekaiju.Utils.EnumArray<Mekaiju.MechaPart, float>;


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MechaPartInstance : EntityInstance
    {
        /// <summary>
        /// 
        /// </summary>
        public MechaInstance mecha { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private MechaPartDesc _desc;

        /// <summary>
        /// 
        /// </summary>
        private float _health;

        /// <summary>
        /// Store the alteration applies to ability (in case we have to revert it).
        /// </summary>
        private IAlteration _alteration;

        private void Awake()
        {
            var t_smr   = GetComponent<SkinnedMeshRenderer>();
            var t_mesh  = t_smr.sharedMesh;
            var t_bones = t_smr.bones;

            HashSet<int> t_usedBonesIndex = new();
            t_mesh.boneWeights.ToList().ForEach(t_bw => {
                t_usedBonesIndex.Add(t_bw.boneIndex0);
                t_usedBonesIndex.Add(t_bw.boneIndex1);
                t_usedBonesIndex.Add(t_bw.boneIndex2);
                t_usedBonesIndex.Add(t_bw.boneIndex3);
            });
            var t_influentBones = t_usedBonesIndex.Where(t_ubi => t_ubi < t_bones.Length).Select(t_ubi => t_bones[t_ubi]);

            t_influentBones.ToList().ForEach(t_bone => {
                if (t_bone.gameObject.TryGetComponent<Collider>(out var _))
                {
                    var t_mcp = t_bone.gameObject.AddComponent<MechaCollisionProxy>();
                    t_mcp.onCollide.AddListener(t_collision => onCollide.Invoke(t_collision));
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_inst"></param>
        /// <param name="p_config"></param>
        public void Initialize(MechaInstance p_inst, MechaPartDesc p_desc)
        {
            mecha  = p_inst;
            parent = p_inst;

            _desc   = p_desc;
            _health = baseHealth;

            _alteration = null;

            _desc.ability.behaviour?.Initialize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_opt"></param>
        /// <returns></returns>
        public IEnumerator TriggerAbility(BodyPartObject p_target, object p_opt)
        {
            if (_desc.ability.behaviour.IsAvailable(this, p_opt))
            {
                mecha.timePoints[TimePoint.LastAbilityTriggered] = Time.time;
                yield return _desc.ability.behaviour.Trigger(this, p_target, p_opt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReleaseAbility()
        {
            _desc.ability.behaviour.Release();
        }

        /// <summary>
        /// Should be used only by MechaInstance or Self.<br/>
        /// This function is used in case of compound damages.
        /// </summary>
        /// <param name="p_damage">The amount of damage to deal.</param>
        public float TakeDamage(float p_damage)
        {
            var t_recover = 0.5f * p_damage;
            if (isAlive)
            {
                var t_taken = Mathf.Min(p_damage, _health);
                _health = Math.Max(0f, _health - p_damage);

                if (_alteration == null && _health < (mecha.desc.partTreshold / 100f) * baseHealth)
                {
                    _alteration = _desc.ability.behaviour.Alter(_desc.ability.healthTuneAlteration);
                }

                return Mathf.Max(t_taken, t_recover);
            }

            return t_recover;
        }

        /// <summary>
        /// Should be used only by MechaInstance or Self.<br/>
        /// This function is used in case of compound heal.
        /// </summary>
        /// <param name="p_amount">The amount of health to restore.</param>
        public float HHeal(float p_amount)
        {
            if (isAlive)
            {
                var t_health = _health;
                _health = Mathf.Min(baseHealth, _health + p_amount);

                if (_alteration != null && _health > (mecha.desc.partTreshold / 100f) * baseHealth)
                {
                    _desc.ability.behaviour.Revert(_alteration);
                    _alteration = null;
                }

                return _health - t_health;
            }

            return 0f;
        }

        public override void Update()
        {
            _desc.ability.behaviour?.Tick(this);
        }

        public override void FixedUpdate()
        {
            _desc.ability.behaviour?.FixedTick(this);
        }

        #region IEntityInstance implementation
        public override List<StatefullEffect> effects => parent.effects;

        public override UnityEvent<StatefullEffect> onAddEffect    => parent.onAddEffect;
        public override UnityEvent<StatefullEffect> onRemoveEffect => parent.onRemoveEffect;

        public override EnumArray<StatisticKind, ModifierCollection> modifiers   => parent.modifiers;
        public override EnumArray<StatisticKind, IStatistic>        statistics => parent.statistics;

        public override EnumArray<TimePoint, float>        timePoints => parent.timePoints;
        public override EnumArray<StateKind, State<bool>> states     => parent.states;

        public override UnityEvent<IDamageable, float, DamageKind> onBeforeTakeDamage => parent.onBeforeTakeDamage;
        public override UnityEvent<IDamageable, float, DamageKind> onAfterTakeDamage  => parent.onAfterTakeDamage;
        public override UnityEvent<float> onDealDamage => parent.onDealDamage;

        public override bool isAlive => health > 0f;

        public override float baseHealth => statistics[StatisticKind.Health].Apply<HealthStatisticValue>(modifiers[StatisticKind.Health])[_desc.part];
        public override float health     => _health;

        public override float Heal(float p_amount)
        {
            var t_heal = HHeal(p_amount);
            mecha.HHeal(t_heal);
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
                var t_taken   = TakeDamage(t_damage);

                mecha.TakeDamage(t_taken);
                onAfterTakeDamage.Invoke(p_from, t_taken, p_kind);
                return t_taken;
            }
            return 0f;
        }

        public override float baseStamina => parent.baseStamina;
        public override float stamina     => parent.stamina;

        public override void ConsumeStamina(float p_amount)
        {
            parent.ConsumeStamina(p_amount);
        }

        public override void RestoreStamina(float p_amount)
        {
            parent.RestoreStamina(p_amount);
        }
    }
#endregion
}
