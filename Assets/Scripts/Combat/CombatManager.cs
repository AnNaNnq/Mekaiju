using UnityEngine;
using Mekaiju.AI;

namespace Mekaiju
{
    public class CombatManager : MonoBehaviour
    {
        public MechaInstance mechaInstance { get; private set; }
        public KaijuInstance kaijuInstance { get; private set; }

        public CombatState  state  { get; private set; }
        public CombatResult result { get; private set; }

        private void Awake()
        {
            GameObject t_go;
            if (t_go = GameObject.FindWithTag("Player"))
            {
                if (t_go.TryGetComponent<MechaInstance>(out var t_inst))
                {
                    mechaInstance = t_inst;
                }
                else
                {
                    Debug.LogWarning("GameObject with tag Player must have MechaInstance script!");
                }
            }
            else
            {
                Debug.LogWarning("A mecha must be present in the scene!");
            }

            if (t_go = GameObject.FindWithTag("Kaiju"))
            {
                if (t_go.TryGetComponent<KaijuInstance>(out var t_inst))
                {
                    kaijuInstance = t_inst;
                }
                else
                {
                    Debug.LogWarning("GameObject with tag Kaiju must have KaijuInstance script!");
                }
            }
            else
            {
                Debug.LogWarning("A kaiju must be present in the scene!");
            }
        }

        private void Start()
        {
            state  = CombatState.Started;
            result = CombatResult.None;

            mechaInstance?.onTakeDamage.AddListener(_OnEntityTakeDamage);
            kaijuInstance?.onTakeDamage.AddListener(_OnEntityTakeDamage);
        }

        private void Oestroy()
        {
            mechaInstance?.onTakeDamage?.RemoveListener(_OnEntityTakeDamage);
            kaijuInstance?.onTakeDamage?.RemoveListener(_OnEntityTakeDamage);           
        }

        private void _OnEntityTakeDamage(float p_damage)
        {
            if (!(mechaInstance.isAlive && kaijuInstance.isAlive))
            {
                state  = CombatState.Ended;
                result = mechaInstance.isAlive ? CombatResult.Win : CombatResult.Loose;
            }
        }
    }

}