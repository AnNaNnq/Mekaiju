using Mekaiju.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace Mekaiju.AI
{

    //Temps avant de follow le joueur lors de l'éloginement + Autre temps avant de retourner au nest
    //Faire Agro
    //Variable dans le script pour la vitesse (du kaju)
    public abstract class BasicAI : MonoBehaviour
    {
        
        private NavMeshAgent _agent;

        public CombatStates states;

        public float agroTriggerArea = 10f;
        public float awaitTriggerArea = 30f;
        public float awaitPlayerDistance = 20f;

        [SerializeField] Transform _nest;
        [SerializeField] private LayerMask _mask;
        [SerializeField] private string _tag;

        private GameObject _target;

        protected void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            states = CombatStates.Normal;
            _target = GameObject.FindGameObjectWithTag(_tag);
        }

        protected void Update()
        {
            ChangeState();
            CombatStatesMachine();
        }

        public void ChangeState()
        {
            if (states != CombatStates.Agro)
            {
                if (FindPlayer(agroTriggerArea))
                {
                    states = CombatStates.Agro;
                }
                else
                {
                    if (FindPlayer(awaitTriggerArea))
                    {
                        if (states != CombatStates.Await)
                        {
                            states = CombatStates.Await;
                        }
                    }
                    else
                    {
                        if (states != CombatStates.Normal)
                        {
                            states = CombatStates.Normal;
                        }
                    }
                }
            }
        }

        public void AIMove()
        {
            if (states == CombatStates.Await)
            {
                _agent.destination = _target.transform.position;
                _agent.stoppingDistance = awaitPlayerDistance;
            }
            else if (states == CombatStates.Normal)
            {
                _agent.SetDestination(_nest.position);
                _agent.stoppingDistance = 0.3f;
            }
            Debug.Log(_agent.destination);
        }

        public void CombatStatesMachine()
        {
            switch (states)
            {
                case CombatStates.Agro: break;
                case CombatStates.Await:
                    var t_targetPoint = new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z) - transform.position;
                    var t_targetRotation = Quaternion.LookRotation(t_targetPoint, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, t_targetRotation, Time.deltaTime * 2.0f);
                    AIMove();
                    break;
                case CombatStates.Normal: AIMove(); break;
                default: break;
            }
        }

        public bool FindPlayer(float p_range)
        {
            Collider[] t_collisions = Physics.OverlapSphere(transform.position, p_range, _mask);
            foreach (Collider t_col in t_collisions)
            {
                if (t_col.CompareTag(_tag))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, agroTriggerArea);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, awaitTriggerArea);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, awaitPlayerDistance);
        }
    }
}
