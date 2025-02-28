using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
public class WeaponBullet : MonoBehaviour
{
    public UnityEvent<Collision> OnCollide;
    private Rigidbody _rb;
    private Vector3 _velocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision p_collision)
    {
        OnCollide.Invoke(p_collision);
    }

    public void Launch(Vector3 p_velocity, Vector3 p_direction)
    {
        _velocity = p_velocity;
        transform.rotation = Quaternion.LookRotation(p_direction);
        transform.Rotate(new (0, 90, 0));
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _velocity;
    }
}
