using Mekaiju.Attribute;
using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Mekaiju.AI
{

    //Temps avant de follow le joueur lors de l'éloginement + Autre temps avant de retourner au nest
    //Faire Agro
    public abstract class BasicAI : MonoBehaviour
    {
        [Foldout("General")]
        public CombatStates states;
        public LayerMask mask;
        [Tag] public string targetTag;

        [Foldout("Agro")]
        [PositiveValueOnly] public float agroTriggerArea = 10f;
        [PositiveValueOnly] public float agroSpeed = 3.5f;

        [Foldout("Await")]
        [PositiveValueOnly] public float awaitTriggerArea = 30f;
        [PositiveValueOnly] public float awaitPlayerDistance = 20f;
        [PositiveValueOnly] public float awaitSpeed = 3f;
        [PositiveValueOnly][Tooltip("Temps avant le changement d'état pour await (en s)")] public float timeBeforeAwait = 2f;

        [Foldout("Normal")]
        [MustBeAssigned] [FocusTransform] public Transform nest;
        [PositiveValueOnly] public float normalSpeed = 3f;
        [PositiveValueOnly] [Tooltip("Temps avant le changement d'état pour normal (en s)")] public float timeBeforeNormal = 2f;

        [Foldout("Debug")]
        public bool showGizmo;

        protected GameObject _target;
        protected NavMeshAgent _agent;

        private bool _canSwitch = false;

        protected void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            states = CombatStates.Normal;
            _target = GameObject.FindGameObjectWithTag(targetTag);
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
                    _agent.speed = agroSpeed;
                }
                else
                {
                    if (FindPlayer(awaitTriggerArea))
                    {
                        if (states != CombatStates.Await)
                        {
                            if (_canSwitch)
                            {
                                states = CombatStates.Await;
                                _agent.speed = awaitSpeed;
                            }
                            else
                            {
                                StartCoroutine(Countdown(timeBeforeAwait));
                            }
                        }
                    }
                    else
                    {
                        if (states != CombatStates.Normal)
                        {
                            if (_canSwitch)
                            {
                                states = CombatStates.Normal;
                                _agent.speed = normalSpeed;
                            }
                            else
                            {
                                StartCoroutine(Countdown(timeBeforeNormal));
                            }
                        }
                    }
                }
            }
        }

        public void AIMove()
        {
            if (states == CombatStates.Await)
            {
                float distanceToTarget = Vector3.Distance(_agent.transform.position, _target.transform.position);

                if (distanceToTarget < awaitPlayerDistance)
                {
                    Vector3 directionAwayFromTarget = (_agent.transform.position - _target.transform.position).normalized;
                    _agent.destination = _agent.transform.position + directionAwayFromTarget * (awaitPlayerDistance - distanceToTarget);
                    _agent.stoppingDistance = 0.3f;
                    Vector3 lookDirection = _target.transform.position - _agent.transform.position;
                    lookDirection.y = 0;
                    _agent.transform.rotation = Quaternion.LookRotation(lookDirection);
                }
                else
                {
                    _agent.destination = _target.transform.position;
                    _agent.stoppingDistance = awaitPlayerDistance;
                }

                _canSwitch = false;
            }

            else if (states == CombatStates.Normal)
            {
                _agent.SetDestination(nest.position);
                _agent.stoppingDistance = 0.3f;
                _canSwitch = false;
            }
        }

        public void CombatStatesMachine()
        {
            switch (states)
            {
                case CombatStates.Agro: Agro(); break;
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
            Collider[] t_collisions = Physics.OverlapSphere(transform.position, p_range, mask);
            foreach (Collider t_col in t_collisions)
            {
                if (t_col.CompareTag(targetTag))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            if(!showGizmo) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, agroTriggerArea);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, awaitTriggerArea);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, awaitPlayerDistance);
        }

        public virtual void Agro() {}

        public IEnumerator Countdown(float p_timer)
        {
            _canSwitch = false;
            yield return new WaitForSeconds(p_timer);
            _canSwitch = true;
        }
    }
}
