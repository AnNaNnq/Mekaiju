using Mekaiju.AI.Attack.Instance;
using System;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class FireZone : MonoBehaviour
    {
        private DoomsdayRayUpgrade _stats;
        private KaijuInstance _kaiju;
        private bool _playerInside = false;

        public void Init(DoomsdayRayUpgrade p_stats, KaijuInstance p_kaiju)
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
                yield return new WaitForSeconds(_stats.fireTickRate);
                float t_dmg = _kaiju.GetRealDamage(_stats.fireDamage);
                p_mech.TakeDamage(t_dmg);

                //Debug
                _kaiju.AddDPS(t_dmg);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_playerInside && other.CompareTag("Player"))
            {
                _playerInside = false;
            }
        }
    }
}