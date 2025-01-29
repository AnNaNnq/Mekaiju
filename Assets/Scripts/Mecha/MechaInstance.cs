using System;
using System.Collections;
using System.Collections.Generic;
using Mekaiju.Utils;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    public class MechaInstance : MonoBehaviour
    {
        public static int DefaultStamina = 100;

        /// <summary>
        /// 
        /// </summary>
        public MechaConfig Config;

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
        private int _stamina;

        private void Start()
        {
            Config = GameManager.Instance.PData.Config;

            Debug.Assert(Config.Base.Prefab);
            var t_main = Instantiate(Config.Base.Prefab, transform);

            _parts = Config.Parts.Select((key, part) => 
                {
                    var t_tr = t_main.transform.FindNested(Enum.GetName(typeof(MechaPart), key) + "Anchor");
                    Debug.Assert(t_tr);

                    t_tr.gameObject.SetActive(false);

                    Debug.Assert(part.Base.DefaultAbility.Prefab);
                    var t_go = Instantiate(part.Base.DefaultAbility.Prefab, t_tr);

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

            _health  = 100;
            _stamina = DefaultStamina;
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
        public bool CanExecuteAbility(int p_consumption)
        {
            return _stamina - p_consumption >= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_consumption"></param>
        /// <param name="p_part"></param>
        /// <param name="p_target"></param>
        /// <returns></returns>
        public IEnumerator ExecuteAbility(int p_consumption, MechaPart p_part, MechaInstance p_target)
        {
            _stamina -= p_consumption;
            yield return _parts[p_part].Ability.Behaviour.Execute(this, p_target);
        }
    }

}
