using System;
using System.Collections;
using System.Collections.Generic;
using Mekaiju.Utils;
using UnityEngine;
using UnityEngine.UIElements;

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
            if (!_playerInside && other.gameObject.TryGetMechaPartInstance(out var t_inst))
            {
                _playerInside = true;
                // MechaPartInstance t_mechaPart = other.GetComponent<MechaPartInstance>();
                StartCoroutine(DealDamage(t_inst));
                if (_stats.addEffect)
                {
                    float t_duration = _stats.asDuration ? _stats.effectDuration : -1;
                    _effect = t_inst.mecha.AddEffect(_stats.effect, t_duration);
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

        private IEnumerator DealDamage(MechaPartInstance p_mech)
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
            if (_playerInside && other.gameObject.TryGetMechaPartInstance(out var t_inst))
            {
                _playerInside = false;
                if (!_stats.asDuration && _effect != null)
                {
                    // MechaPartInstance t_mechaPart = other.GetComponent<MechaPartInstance>();
                    t_inst.mecha.RemoveEffect(_effect);
                }
            }
        }
    }
}