using UnityEngine;

namespace Mekaiju.Destruction
{
    public class ObjectDestruction : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.CompareTag("Player"))
            {
                Rigidbody t_playerRb = collision.collider.GetComponent<Rigidbody>();
                float t_force = Mathf.Round(t_playerRb.linearVelocity.magnitude * 100f) / 100f;
                Debug.Log(t_force);
            }
        }
    }
}

