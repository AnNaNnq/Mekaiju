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
        Collider[] _cols;
        Rigidbody _rb;

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

        void FixedUpdate()
        {
            if(_radius <= _stats.maxRadius)
            {
                SetPosition();
                ApplyForce();
                _radius += Time.deltaTime * _stats.shockwaveSpeed;
                _lr.widthMultiplier = (_stats.maxRadius - _radius) / _stats.maxRadius;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void ApplyForce()
        {
            _cols = Physics.OverlapSphere(transform.position, _radius);
            foreach (var col in _cols)
            {
                if (col.CompareTag("Player"))
                {
                    Rigidbody rb = col.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 direction = (col.transform.position - transform.position).normalized;
                        Vector3 force = direction * _stats.repulsionForce + Vector3.up * _stats.verticalForce;

                        rb.AddForce(force, ForceMode.Impulse);
                    }
                }
            }
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
