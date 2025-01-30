using UnityEngine;
using UnityEngine.Events;

public class WeaponBullet : MonoBehaviour
{
    public UnityEvent<Collision> OnCollide;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollide.Invoke(collision);
    }

    public void Launch(Vector3 velocity)
    {
        _rb.linearVelocity = velocity;
    }
}
