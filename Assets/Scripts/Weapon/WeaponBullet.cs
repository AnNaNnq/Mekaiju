using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
public class WeaponBullet : MonoBehaviour
{
    public UnityEvent<Collision> OnCollide;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision p_collision)
    {
        OnCollide.Invoke(p_collision);
    }

    public void Launch(Vector3 p_velocity, Vector3 p_direction)
    {
        _rb.linearVelocity = p_velocity;
        transform.rotation = Quaternion.LookRotation(p_direction);
        transform.Rotate(new (0, 90, 0));
    }
}
