using UnityEngine;
using UnityEngine.AI;

public abstract class BasicAI : MonoBehaviour
{
    private NavMeshAgent _agent;
    public States states;

    protected void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        states = States.Normal;
    }



}
