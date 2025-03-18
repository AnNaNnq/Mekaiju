using System.Collections.Generic;
using UnityEngine;

namespace Mekaiju.AI.Attack.Instance
{
    public class ShockWave : MonoBehaviour
    {
        ChockWave _stats;

        LineRenderer _lr;
        float _radius = 0;
        float _lineWith = 3;
        int _pointCount = 30;
        float _angleBetweenPoint = 0;
        Vector3[] _positions = new Vector3[30];
        Collider[] _cols;
        Rigidbody _rb;

        HashSet<Collider> _hitObjects = new HashSet<Collider>();

        public void SetUp(ChockWave p_stats)
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
            _cols = Physics.OverlapSphere(transform.position, _radius + 0.5f); // Légèrement plus large

            foreach (var col in _cols)
            {
                if (col.CompareTag("Player") && !_hitObjects.Contains(col)) // Vérifie si pas déjà touché
                {
                    Vector3 t_toPlayer = col.transform.position - transform.position;
                    float t_distanceToWave = Mathf.Abs(t_toPlayer.magnitude - _radius); // Distance exacte

                    if (t_distanceToWave < _lineWith) // Vérifie si le joueur est bien sur la ligne de l'onde
                    {
                        _hitObjects.Add(col); // Ajoute l'objet pour éviter de le toucher plusieurs fois
                        Rigidbody t_rb = col.GetComponent<Rigidbody>();
                        MechaInstance t_instance = col.GetComponent<MechaInstance>();
                        if (t_rb != null && t_instance.states[Entity.State.Grounded])
                        {
                            t_instance.AddEffect(_stats.effect, _stats.effectDuration);
                            _stats.SendDamage(_stats.shockwaveDamage, t_instance, _stats.effect, _stats.effectDuration);
                        }
                    }
                }
            }
        }

        void SetPosition()
        {
            for (int i = 0; i < _pointCount; i++)
            {
                Vector3 worldPoint = transform.position + (_positions[i] * _radius); // Convertir en coordonnées mondiales
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
