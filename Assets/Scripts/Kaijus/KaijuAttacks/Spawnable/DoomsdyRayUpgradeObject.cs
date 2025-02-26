using TMPro;
using UnityEngine;

namespace Mekaiju.AI
{
    public class DoomsdyRayUpgradeObject : MonoBehaviour
    {
        Rigidbody _rb;

        DoomsdayRayUpgrade _stat;

        public void Init(DoomsdayRayUpgrade p_stat, Vector3 p_target)
        {
            _stat = p_stat;
            _rb = GetComponent<Rigidbody>();

            Vector3 direction = (p_target - transform.position);
            float distance = direction.magnitude;
            float gravity = Physics.gravity.magnitude;

            float angle = 45f * Mathf.Deg2Rad; // Angle de tir (modifiable)
            float velocity = Mathf.Sqrt(distance * gravity / Mathf.Sin(2 * angle)) * _stat.speed;

            Vector3 velocityVector = direction.normalized * Mathf.Cos(angle) * velocity + Vector3.up * Mathf.Sin(angle) * velocity;
            _rb.linearVelocity = velocityVector;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision.gameObject.tag);

            if (collision.gameObject.CompareTag("Bouncable"))
            {
                Vector3 reflectDirection = Vector3.Reflect(_rb.linearVelocity, collision.contacts[0].normal);

                // Appliquer un rebond normal
                _rb.linearVelocity = reflectDirection * _stat.bounceDamping;

                // Ajouter une impulsion pour renforcer l'effet
               _rb.AddForce(reflectDirection * _stat.bounceForce, ForceMode.Impulse);
            }
        }

    }
}
