using Mekaiju;
using System;
using System.Collections.Generic;
using UnityEngine;

public class KaijuCollsionDetector : MonoBehaviour
{
    public event Action<MechaInstance> OnMechaEnter;
    public event Action<MechaInstance> OnMechaExit;

    public event Action OnGround;

    private void OnTriggerEnter(Collider other)
    {
        MechaInstance mecha = other.GetComponent<MechaInstance>();
        if (mecha != null)
        {
            OnMechaEnter?.Invoke(mecha);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Walkable"))
        {
            OnGround?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MechaInstance mecha = other.GetComponent<MechaInstance>();
        if (mecha != null)
        {
            OnMechaExit?.Invoke(mecha);
        }
    }
}
