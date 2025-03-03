using UnityEngine;

namespace Mekaiju.AI.Attack.Instance
{
    public class DoomsdayRayUpgradeObject : MonoBehaviour
    {
        DoomsdayRayUpgrade _stat;

        [SerializeField]
        private LineRenderer _lr;

        private Transform _startPoint;
        public LayerMask layerMask;

        public void Init(DoomsdayRayUpgrade p_stat)
        {
            _stat = p_stat;
            _startPoint = transform;

            _lr = GetComponent<LineRenderer>();
            _lr.SetPosition(0, _startPoint.position);
            _lr.positionCount = _stat.maxBounce + 1;
        }

        private void Update()
        {
            CastLaser(transform.position, transform.forward + (-transform.up));
        }

        void CastLaser(Vector3 p_position, Vector3 p_direction)
        {
            _lr.SetPosition(0, _startPoint.position);

            for (int i = 0; i < _stat.maxBounce; i++)
            {
                Ray ray = new Ray(p_position, p_direction);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
                {
                    p_position = hit.point;
                    p_direction = Vector3.Reflect(p_direction, hit.normal);
                    _lr.SetPosition(i + 1, hit.point);
                }
            }
        }
    }
}
