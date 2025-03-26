using Mekaiju.AI;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class KaijuMotor : MonoBehaviour
{
    private KaijuInstance _instance;

    public NavMeshAgent agent { get { return _agent; } }
    private NavMeshAgent _agent;

    private GameObject _target;

    void Start()
    {
        _instance = GetComponent<KaijuInstance>();
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.FindGameObjectWithTag(_instance.targetTag);
    }

    /// <summary>
    /// Makes the Kaiju move backwards while looking at the player
    /// </summary>
    /// <param name="p_pos"></param>
    /// <param name="p_stopping"></param>
    public void BackOff(Vector3 p_pos, float p_speed, float p_stopping = 10f)
    {
        SetSpeed(p_speed);
        MoveTo(p_pos, p_stopping);
        LookTarget();
    }

    public void MoveTo(Vector3 p_pos)
    {
        MoveTo(p_pos, _instance.stoppingDistanceMin);
    }

    /// <summary>
    /// Moves the Kaiju to a given position
    /// </summary>
    /// <param name="p_pos"></param>
    /// <param name="p_stopping"></param>
    public void MoveTo(Vector3 p_pos, float p_stopping)
    {
        if (_agent.enabled == false) return;
        p_stopping = Mathf.Max(p_stopping, _instance.stoppingDistanceMin);

        
        _agent.destination = p_pos;
        _agent.stoppingDistance = p_stopping;
    }

    public void SetSpeed(float p_speed)
    {
        _agent.speed = _instance.GetRealSpeed(p_speed);
    }

    /// <summary>
    /// Forces the Kaiju to look at the player
    /// </summary>
    public void LookTarget()
    {
        Vector3 t_direction = _target.transform.position - transform.position;
        t_direction.y = 0; // Ignore the Y component to avoid tilting

        // Check that direction is not zero to avoid errors
        if (t_direction != Vector3.zero)
        {
            Quaternion t_targetRotation = Quaternion.LookRotation(t_direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, t_targetRotation, _agent.angularSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Recover the position behind the Kaiju
    /// </summary>
    /// <param name="p_distance"></param>
    /// <returns></returns>
    public Vector3 GetPositionBehind(float p_distance)
    {
        // Take the target direction and reverse it
        Vector3 direction = (transform.position - _target.transform.position).normalized;
        return transform.position + direction * p_distance;
    }

    public void StopKaiju(float p_time)
    {
        if (!_agent.enabled) return;
        StartCoroutine(Stop(p_time));
    }

    public void StopKaiju()
    {
        if (!_agent.enabled) return;
        _agent.ResetPath();
        _agent.enabled = false;
    }

    public void StartKaiju()
    {
        _agent.enabled = true;
    }

    private IEnumerator Stop(float p_time)
    {
        _agent.ResetPath();
        _agent.enabled = false;
        yield return new WaitForSeconds(p_time);
        _agent.enabled = true;
    }

    public bool IsInMovement()
    {
        return agent.velocity.magnitude > 0.1f && !agent.isStopped;
    }
}
