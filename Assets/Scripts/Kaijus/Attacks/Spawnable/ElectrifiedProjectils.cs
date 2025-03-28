﻿using Mekaiju.Utils;
using UnityEngine;

namespace Mekaiju.AI.Attack.Instance
{
    public class ElectrifiedProjectils : MonoBehaviour
    {
        float _dmg;
        ElectrifiedSparks _stat;

        private void Start()
        {
            Destroy(gameObject, 10);
        }

        public void Init(ElectrifiedSparks p_stat)
        {
            KaijuInstance t_kaiju = GameObject.FindGameObjectWithTag("Kaiju").GetComponent<KaijuInstance>();
            _dmg = t_kaiju.GetRealDamage(p_stat.damage);
            _stat = p_stat;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetMechaPartInstance(out var t_inst))
            {
                KaijuInstance t_kaiju = GameObject.FindGameObjectWithTag("Kaiju").GetComponent<KaijuInstance>();
                t_inst.TakeDamage(t_kaiju, _dmg, Entity.DamageKind.Direct);
                t_inst.mecha.AddEffect(_stat.effect, _stat.effectDuration);
            }
            if(!other.CompareTag("KaijuPart") && !other.CompareTag("DoomsdayRaySpawn") && !other.CompareTag("Spawnable") && !other.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }
}
