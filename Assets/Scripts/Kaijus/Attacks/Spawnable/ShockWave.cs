using UnityEngine;

namespace Mekaiju.AI.Attack.Instance
{
    public class ShockWave : MonoBehaviour
    {
        GroundStrike _stats;

        LineRenderer _lr;
        float _radius = 0;
        float _lineWith = 3;
        int _pointCount = 30;
        float _angleBetweenPoint = 0;
        Vector3[] _positions = new Vector3[30];

        public void SetUp(GroundStrike p_stats)
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

        void Update()
        {
            SetPosition();
            _radius += Time.deltaTime * _stats.shockwaveSpeed;
        }

        void SetPosition()
        {
            for (int i = 0; i < _pointCount; i++)
            {
                _lr.SetPosition(i, _positions[i] * _radius);
            }
        }
    }
}
