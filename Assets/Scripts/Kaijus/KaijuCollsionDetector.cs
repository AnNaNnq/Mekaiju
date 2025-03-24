using Mekaiju;
using System;
using UnityEngine;

public class KaijuCollsionDetector : MonoBehaviour
{
    public event Action<MechaPartInstance> OnMechaEnter;
    public event Action<MechaPartInstance> OnMechaExit;

    public event Action OnGround;

    public event Action OnShieldEnter;
    public event Action OnShieldExit;

    private void OnTriggerEnter(Collider other)
    {
        MechaPartInstance mecha = other.GetComponent<MechaPartInstance>();
        if (mecha != null)
        {
            OnMechaEnter?.Invoke(mecha);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Walkable"))
        {
            OnGround?.Invoke();
        }

        if (other.CompareTag("Shield"))
        {
            OnShieldEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MechaPartInstance mecha = other.GetComponent<MechaPartInstance>();
        if (mecha != null)
        {
            OnMechaExit?.Invoke(mecha);
        }

        if (other.CompareTag("Shield"))
        {
            OnShieldExit?.Invoke();
        }
    }
}
