using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class WeaponBullet : MonoBehaviour
{
    public UnityEvent<GameObject, Collision> OnCollide;

    private Rigidbody _rb;
    private Vector3   _velocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision p_collision)
    {
        OnCollide.Invoke(gameObject, p_collision);
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _velocity;
    }

    /// <summary>
    /// Launch the bullet with the given speed and given direction
    /// </summary>
    /// <param name="p_velocity">The target velocity</param>
    /// <param name="p_direction">The target direction</param>
    public void Launch(Vector3 p_velocity, Vector3 p_direction)
    {
        _velocity = p_velocity;
        transform.rotation = Quaternion.LookRotation(p_direction);
        transform.Rotate(new (0, 90, 0));
    }

    /// <summary>
    /// Destroy the projectile after the given timout
    /// </summary>
    /// <param name="p_timout">The max time the projectile can stay alive</param>
    public void Timout(float p_timout)
    {
        Destroy(gameObject, p_timout);
    }
}
