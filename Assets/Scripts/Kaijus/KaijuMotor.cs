using Mekaiju.AI;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(KaijuInstance))]
[RequireComponent(typeof(NavMeshAgent))]
public class KaijuMotor : MonoBehaviour
{
    private KaijuInstance _instance;
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
    public void BackOff(Vector3 p_pos, float p_speed, float p_stopping = 0.2f)
    {
        MoveTo(p_pos, p_stopping);
        LookTarget();
    }


    /// <summary>
    /// Moves the Kaiju to a given position
    /// </summary>
    /// <param name="p_pos"></param>
    /// <param name="p_stopping"></param>
    public void MoveTo(Vector3 p_pos, float p_speed, float p_stopping = 10f)
    {
        if (!_agent.enabled) return;
        p_stopping = Mathf.Max(p_stopping, 10f);

        float t_speed = _instance.stats.speed * (1 + (p_speed / 100));

        _agent.speed = t_speed;
        _agent.destination = p_pos;
        _agent.stoppingDistance = p_stopping;
    }

    public void Stop()
    {
        _agent.ResetPath();
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

    public void StopKaiju(float p_time)
    {
        StartCoroutine(Stop(p_time));
    }

    private IEnumerator Stop(float p_time)
    {
        _agent.ResetPath();
        _agent.enabled = false;
        yield return new WaitForSeconds(p_time);
        _agent.enabled = true;
    }
}
