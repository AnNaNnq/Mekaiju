using System;
using System.Collections;
using System.Collections.Generic;
using Mekaiju.AI;
using Mekaiju.Utils;
using UnityEngine;

namespace Mekaiju
{
    [Serializable]
    public class InstanceContext
    {
        public float LastAbilityTime = -1000f;
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
        private List<EffectState> _effects;
        public  List<EffectState> Effetcs => _effects;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _health;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _stamina;

        /// <summary>
        /// 
        /// </summary>
        public InstanceContext Context { get; private set; }

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
                    t_inst.Initialize(part);

                    t_tr.gameObject.SetActive(true);

                    return t_inst;
                }
            );

            // TODO: remove
            t_main.SetActive(false);

            // _effects = new();
            _effects = new()
            {
                new(Resources.Load<Effect>("Mecha/Effect/Stamina")),
                new(Resources.Load<Effect>("Mecha/Effect/Bleeding"))
            };

            _health  = Desc.Health;
            _stamina = Desc.Stamina;

            Context = new();
        }

        private void Update()
        {
            List<int> t_toRemove = new();
            for (int i = 0; i < _effects.Count; i++)
            {
                var t_effect = _effects[i];
                t_effect.Tick(this);
                if (t_effect.ToRemove)
                    t_toRemove.Add(i);
            }
            
            for (int i = 0; i < t_toRemove.Count; i++)
            {
                _effects.RemoveAt(t_toRemove[i]);
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return _health > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_damage"></param>
        public void TakeDamage(float p_damage)
        {
            _health = Math.Max(0, _health - p_damage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_part"></param>
        public void SwapAbility(MechaPart p_part)
        {
            _parts[p_part].SwapAbility();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_consumption"></param>
        /// <returns></returns>
        public bool CanExecuteAbility(float p_consumption)
        {
            return _stamina - p_consumption > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void RestoreStamina(float p_amount)
        {
            _stamina = Math.Min(Desc.Stamina, _stamina + p_amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void ConsumeStamina(float p_amount)
        {
            _stamina = Math.Max(0, _stamina - p_amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_consumption"></param>
        /// <param name="p_part"></param>
        /// <param name="p_target"></param>
        /// <returns></returns>
        public IEnumerator ExecuteAbility(MechaPart p_part, BasicAI p_target, object p_opt)
        {
            if (CanExecuteAbility(_parts[p_part].Ability.Behaviour.Consumption(p_opt)))
            {
                Context.LastAbilityTime = Time.time;
                yield return _parts[p_part].Ability.Behaviour.Trigger(this, p_target, p_opt);
            }
        }
    }

}
