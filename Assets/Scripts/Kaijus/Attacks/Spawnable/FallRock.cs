using Mekaiju.Utils;
using UnityEngine;

namespace Mekaiju.AI.Attack.Instance
{
    public class FallRock : MonoBehaviour
    {
        public float _dmg;
        AbyssalVortex _stat;
        public GameObject impactVFX;
        KaijuInstance _instance;


        public void SetUp(AbyssalVortex p_stat, KaijuInstance p_instance)
        {
            _stat = p_stat;
            _dmg = p_instance.stats.dmg * (1 + (_stat.damage / 100));
            _instance = p_instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetMechaPartInstance(out var t_inst))
            {
                var t_dmg = _instance.GetRealDamage(_dmg);
                t_inst.TakeDamage(_instance, t_dmg, Entity.DamageKind.Direct);
                _instance.AddDPS(t_dmg);
                _instance.UpdateUI();
            }
            else
            {
                GameObject t_impactVFX = Instantiate(impactVFX, transform.position, Quaternion.identity);
                Destroy(t_impactVFX, 2);
                Destroy(gameObject);
            }
        }
    }
}
