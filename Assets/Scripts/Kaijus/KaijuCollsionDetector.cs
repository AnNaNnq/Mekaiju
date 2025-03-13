using Mekaiju;
using System;
using System.Collections.Generic;
using UnityEngine;

public class KaijuCollsionDetector : MonoBehaviour
{
    public event Action<MechaInstance> OnMechaEnter;
    public event Action<MechaInstance> OnMechaExit;

    private void OnTriggerEnter(Collider other)
    {
        MechaInstance mecha = other.GetComponent<MechaInstance>();
        if (mecha != null)
        {
            OnMechaEnter?.Invoke(mecha);
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
