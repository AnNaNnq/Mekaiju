using System;
using UnityEngine;

namespace Mekaiju.Utils
{
    public class ChildCollider : MonoBehaviour
    {
        public event Action<Collider> OnChildTriggeredEnter;
        public event Action<Collider> OnChildTriggeredExit;

        private void OnTriggerEnter(Collider other)
        {
            OnChildTriggeredEnter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnChildTriggeredExit?.Invoke(other);
        }
    }
}
