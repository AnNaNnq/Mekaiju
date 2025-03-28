using Mekaiju.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mekaiju.AI.Attack.Instance
{
    public class DoomsdayRayUpgradeObject : MonoBehaviour
    {
        DoomsdayRayUpgrade _stat;

        List<Vector3> _firePos = new List<Vector3>();

        KaijuInstance _instance;
        private Transform _startPoint;
        public LayerMask layerMask;

        public void Init(DoomsdayRayUpgrade p_stat, KaijuInstance p_kaiju)
        {
            _stat = p_stat;
            _startPoint = transform;
            _instance = p_kaiju;

            _firePos.Clear();
            UtilsFunctions.CastLaser(transform.position, transform.forward + (-transform.up), _firePos, _startPoint, _stat.maxBounce, layerMask);

            StartCoroutine(FollowPath());
        }

        


        IEnumerator FollowPath()
        {
            if (_firePos.Count == 0)
            {
                Debug.LogError("Error: No valid fire positions in _firePos.");
                Destroy(gameObject);
                yield break;
            }

            foreach (Vector3 target in _firePos)
            {
                if (float.IsNaN(target.x) || float.IsNaN(target.y) || float.IsNaN(target.z))
                {
                    Debug.LogError("Error: Invalid target position in _firePos: " + target);
                    yield break;
                }

                Vector3 start = transform.position;
                float journey = 0f;

                while (journey < 1f)
                {
                    float distance = Vector3.Distance(start, target);
                    if (distance == 0)
                    {
                        Debug.LogWarning("Warning: Zero distance between positions, skipping.");
                        break;
                    }

                    journey += Time.deltaTime * _stat.speed / distance;
                    transform.position = Vector3.Lerp(start, target, journey);
                    yield return null;
                }

                transform.position = target; // Ensure exact final position
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetMechaPartInstance(out var t_inst))
            {
                t_inst.TakeDamage(_instance, _instance.GetRealDamage(_stat.damage), Entity.DamageKind.Direct);
            }
            else if (other.CompareTag("Ground"))
            {
                GameObject t_obj = Instantiate(_stat.fireZonePrefab, other.ClosestPoint(transform.position), Quaternion.identity);
                DamageZone t_zone = t_obj.GetComponent<DamageZone>();
                t_zone.Init(_stat.fireZone, _instance);
            }
        }
    }
}
