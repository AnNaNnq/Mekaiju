using System;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class DamageZone : MonoBehaviour
    {
        private DamageZoneStats _stats;
        private KaijuInstance _kaiju;
        private bool _playerInside = false;

        IDisposable _effect;
        public void Init(DamageZoneStats p_stats, KaijuInstance p_kaiju)
        {
            _stats = p_stats;
            _kaiju = p_kaiju;
            _playerInside = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_playerInside && other.CompareTag("Player"))
            {
                _playerInside = true;
                MechaInstance t_mecha = other.GetComponent<MechaInstance>();
                StartCoroutine(DealDamage(t_mecha));
                if (_stats.addEffect)
                {
                    float t_duration = _stats.asDuration ? _stats.effectDuration : -1;
                    _effect = t_mecha.AddEffect(_stats.effect, t_duration);
                }
            }

            //Pour �viter que les zones de feu ne se superposent
            if (other.CompareTag("Spawnable"))
            {
                if (GetInstanceID() > other.GetInstanceID())
                {
                    Destroy(gameObject);
                }
            }
        }

        private IEnumerator DealDamage(MechaInstance p_mech)
        {
            while (_playerInside)
            {
                yield return new WaitForSeconds(_stats.tickRate);
                float t_dmg = _kaiju.GetRealDamage(_stats.damage);
                p_mech.TakeDamage(_kaiju, t_dmg, Entity.DamageKind.Direct);

                //Debug
                _kaiju.AddDPS(t_dmg);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_playerInside && other.CompareTag("Player"))
            {
                _playerInside = false;
                if (!_stats.asDuration && _effect != null)
                {
                    MechaInstance t_mecha = other.GetComponent<MechaInstance>();
                    t_mecha.RemoveEffect(_effect);
                }
            }
        }
    }
}