using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mekaiju.AI.Attack.Instance
{
    public class DoomsdayRayUpgradeObject : MonoBehaviour
    {
        DoomsdayRayUpgrade _stat;

        List<Vector3> _firePos = new List<Vector3>();

        private Transform _startPoint;
        public LayerMask layerMask;

        public void Init(DoomsdayRayUpgrade p_stat)
        {
            _stat = p_stat;
            _startPoint = transform;

            _firePos.Clear();
            CastLaser(transform.position, transform.forward + (-transform.up));

            StartCoroutine(FollowPath());
        }

        void CastLaser(Vector3 p_position, Vector3 p_direction)
        {
            _firePos.Add(_startPoint.position);
            bool hitDetected = false;

            for (int i = 0; i < _stat.maxBounce; i++)
            {
                Ray ray = new Ray(p_position, p_direction);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
                {
                    p_position = hit.point;
                    p_direction = Vector3.Reflect(p_direction, hit.normal);
                    _firePos.Add(hit.point);
                    hitDetected = true;
                }
                else
                {
                    Debug.LogError("Warning: Raycast did not hit anything!");
                    break;
                }
            }

            if (!hitDetected)
            {
                Debug.LogError("Error: No valid hits detected for laser path.");
            }
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
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player");
            }
            else if (other.CompareTag("Ground"))
            {
                Instantiate(_stat.fireZone, other.ClosestPoint(transform.position), Quaternion.identity);
            }
        }
    }
}
