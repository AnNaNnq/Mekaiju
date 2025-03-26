using System.Collections.Generic;
using Mekaiju.Entity;
using UnityEngine;

namespace Mekaiju.AI.Attack.Instance
{
    public class ShockWaveObject : MonoBehaviour
    {
        ShockWaveStat _stats;

        LineRenderer _lr;
        float _radius = 0;
        float _lineWith = 3;
        int _pointCount = 30;
        float _angleBetweenPoint = 0;
        Vector3[] _positions = new Vector3[30];
        Collider[] _cols;
        Rigidbody _rb;

        HashSet<Collider> _hitObjects = new HashSet<Collider>();

        public void SetUp(ShockWaveStat p_stats)
        {
            _stats = p_stats;
            _lr = GetComponent<LineRenderer>();
            _lr.enabled = true;
            _lr.positionCount = _pointCount;

            FindPoint();
        }

        void FindPoint()
        {
            _angleBetweenPoint = 360 / _pointCount;

            for (int i = 0; i < _pointCount - 1; i++)
            {
                float t_angle = _angleBetweenPoint * i * Mathf.Deg2Rad;
                _positions[i] = new Vector3(Mathf.Sin(t_angle), 0, Mathf.Cos(t_angle));
            }
            _positions[_pointCount - 1] = _positions[0];
        }

        void FixedUpdate()
        {
            if(_radius <= _stats.maxRadius)
            {
                SetPosition();
                TriggerColision();
                _radius += Time.deltaTime * _stats.shockwaveSpeed;
                _lr.widthMultiplier = (_stats.maxRadius - _radius) / _stats.maxRadius;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void TriggerColision()
        {
            _cols = Physics.OverlapSphere(transform.position, _radius + 0.5f); // L�g�rement plus large

            foreach (var col in _cols)
            {
                if (col.CompareTag("MechaPart") && !_hitObjects.Contains(col)) // V�rifie si pas d�j� touch�
                {
                    Vector3 t_toPlayer = col.transform.position - transform.position;
                    float t_distanceToWave = Mathf.Abs(t_toPlayer.magnitude - _radius); // Distance exacte

                    if (t_distanceToWave < _lineWith) // V�rifie si le joueur est bien sur la ligne de l'onde
                    {
                        _hitObjects.Add(col); // Ajoute l'objet pour �viter de le toucher plusieurs fois
                        Rigidbody t_rb = col.GetComponent<Rigidbody>();
                        MechaPartInstance t_mechaPart = col.GetComponent<MechaPartInstance>();
                        if (t_rb != null && t_mechaPart.states[StateKind.Grounded])
                        {
                            float t_damage = _stats.kaiju.GetRealDamage(_stats.shockwaveDamage);
                            t_mechaPart.TakeDamage(_stats.kaiju, t_damage, DamageKind.Direct);
                            t_mechaPart.mecha.AddEffect(_stats.effect, _stats.effectDuration);
                        }
                    }
                }
            }
        }

        void SetPosition()
        {
            for (int i = 0; i < _pointCount; i++)
            {
                Vector3 worldPoint = transform.position + (_positions[i] * _radius); // Convertir en coordonn�es mondiales
                RaycastHit hit;

                if (Physics.Raycast(worldPoint + Vector3.up * 5f, Vector3.down, out hit, 10f)) // Raycast vers le bas
                {
                    worldPoint.y = hit.point.y; // Ajuster la hauteur selon le sol
                }

                _lr.SetPosition(i, worldPoint);
            }
        }
    }
}
