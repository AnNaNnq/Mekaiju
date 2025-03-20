using Mekaiju.AI.Attack.Instance;
using Mekaiju.Attribute;
using Mekaiju.Entity;
using Mekaiju.Entity.Effect;
using MyBox;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class ElectrifiedSparks : Attack
    {
        [Separator]
        public int nbProjectilInWave = 8;
        public int nbWave = 3;
        public float timeBetweenWave = 3;
        public float forceAmount = 500f;
        public GameObject projectilPrefab;
        [SOSelector] public Effect effect;
        public float effectDuration = 2.5f;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            _kaiju.motor.StopKaiju();

            _kaiju.animator.AttackAnimation("Rotation");
            _kaiju.StartCoroutine(AttackEnumerator());
        }

        public override IEnumerator AttackEnumerator()
        {
            base.AttackEnumerator();
            int t_waveCount = 0;
            yield return new WaitForSeconds(1);
            while(t_waveCount < nbWave)
            {
                SpawnObjects(nbProjectilInWave, 3f);
                yield return new WaitForSeconds(timeBetweenWave);
                t_waveCount++;
            }
            SpawnObjects(nbProjectilInWave / 2, 3f);
            _kaiju.animator.AttackAnimation("ElectrifiedEnd");
        }

        void SpawnObjects(int p_objectCount, float p_radius)
        {
            for (int i = 0; i < p_objectCount; i++)
            {
                // Calcul de l'angle pour répartir les objets en cercle
                float t_angle = i * (360f / p_objectCount);
                Vector3 t_spawnPosition = _kaiju.transform.position + new Vector3(Mathf.Cos(t_angle * Mathf.Deg2Rad), 0, Mathf.Sin(t_angle * Mathf.Deg2Rad)) * p_radius;

                // Instanciation de l'objet
                GameObject t_newObj = GameObject.Instantiate(projectilPrefab, t_spawnPosition, Quaternion.identity);
                
                ElectrifiedProjectils t_proj = t_newObj.GetComponent<ElectrifiedProjectils>();
                t_proj.Init(this);

                // Calcul de la direction d'expulsion
                Vector3 t_direction = (t_spawnPosition - _kaiju.transform.position).normalized;

                // Rotation de l'objet pour qu'il regarde vers l'extérieur
                t_newObj.transform.rotation = Quaternion.LookRotation(t_direction);

                // Récupération du Rigidbody
                Rigidbody t_rb = t_newObj.GetComponent<Rigidbody>();
                if (t_rb != null)
                {
                    t_rb.AddForce(t_direction * forceAmount, ForceMode.Impulse);
                }
            }
        }

        public float GetRealDamage(float p_amonunt)
        {
            return _kaiju.GetRealDamage(p_amonunt);
        }
    }
}
