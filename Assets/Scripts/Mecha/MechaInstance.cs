using System;
using System.Collections;
using System.Collections.Generic;
using Mekaiju.AI;
using Mekaiju.Utils;
using UnityEngine;

namespace Mekaiju
{

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
        private List<Effect> _effects;

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

            _effects = new();
            // _effects = new()
            // {
            //     Resources.LoadAll<Effect>("Effect/")[0]
            // };
            _health  = Desc.Stamina;
            _stamina = Desc.Health;

        }

        private void Update()
        {
            _effects.ForEach(effect => effect.Behaviour.Tick(this));
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
            return _stamina + p_consumption <= Desc.Stamina;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void ConsumeStamina(float p_amount)
        {
            _stamina = Math.Min(Desc.Stamina, _stamina + p_amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void RestoreStamina(float p_amount)
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
                yield return _parts[p_part].Ability.Behaviour.Execute(this, p_target, p_opt);
            }
        }
    }

}
