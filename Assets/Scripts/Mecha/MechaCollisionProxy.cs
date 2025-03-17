using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju
{
    public class MechaCollisionProxy : MonoBehaviour
    {
        public UnityEvent<Collider> onCollide = new();

        void OnTriggerEnter(Collider p_collider)
        {
            onCollide.Invoke(p_collider);
        }
    }
}