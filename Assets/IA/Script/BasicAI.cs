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
        private NavMeshAgent _agent;

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
        private bool _switchToAwait = false;
        [PositiveValueOnly][Tooltip("Temps avant le changement d'état pour await (en s)")] public float timeBeforeAwait = 2f;

        [Foldout("Normal")]
        [MustBeAssigned] [FocusTransform] public Transform nest;
        [PositiveValueOnly] public float normalSpeed = 3f;
        [PositiveValueOnly] [Tooltip("Temps avant le changement d'état pour normal (en s)")] public float timeBeforeNormal = 2f;

        [Foldout("Debug")]
        public bool showGizmo;

        private GameObject _target;

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
                            states = CombatStates.Await;
                            _agent.speed = awaitSpeed;
                        }
                    }
                    else
                    {
                        if (states != CombatStates.Normal)
                        {
                            states = CombatStates.Normal;
                            _agent.speed = normalSpeed;
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
                _agent.SetDestination(nest.position);
                _agent.stoppingDistance = 0.3f;
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

        public IEnumerator countDown(float p_timer, bool p_var)
        {
            p_var = false;
            yield return new WaitForSeconds(p_timer);
            p_var = true;
        }
    }
}
