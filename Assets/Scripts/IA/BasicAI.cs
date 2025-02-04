using Mekaiju.Attribute;
using MyBox;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Mekaiju.AI
{

    //Temps avant de follow le joueur lors de l'éloginement + Autre temps avant de retourner au nest
    //Faire Agro
    public abstract class BasicAI : MonoBehaviour
    {
        [Foldout("General")]
        public CombatStatesKaiju states;
        public LayerMask mask;
        [Tag] public string targetTag;
        public BodyPart[] bodyParts;

        [Foldout("Agro")]
        [PositiveValueOnly] public float agroTriggerArea = 10f;
        [PositiveValueOnly] public float agroSpeed = 3.5f;

        [Foldout("Await")]
        [PositiveValueOnly] public float awaitTriggerArea = 30f;
        [PositiveValueOnly] public float awaitPlayerDistance = 20f;
        [PositiveValueOnly] public float awaitSpeed = 3f;
        [PositiveValueOnly][Tooltip("Temps avant le changement d'état pour await (en s)")] public float timeBeforeAwait = 2f;

        [Foldout("Normal")]
        [MustBeAssigned] [FocusObject] public Transform nest;
        [PositiveValueOnly] public float normalSpeed = 3f;
        [PositiveValueOnly] [Tooltip("Temps avant le changement d'état pour normal (en s)")] public float timeBeforeNormal = 2f;

        protected GameObject _target;

        protected NavMeshAgent _agent;

        private bool _canSwitch = false;

        [Foldout("Debug")]
        [OverrideLabel("Show Gizmo For Non-agro Phase")]
        public bool showGizmo;
        public TextMeshProUGUI textDPS;


        private int _dps = 0;

        protected void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            states = CombatStatesKaiju.Normal;
            _target = GameObject.FindGameObjectWithTag(targetTag);
            StartCoroutine(ShowDPS());
            textDPS.text = _dps.ToString();
        }

        protected void Update()
        {
            ChangeState();
            CombatStatesMachine();
        }

        public void ChangeState()
        {
            if (states != CombatStatesKaiju.Agro)
            {
                if (FindPlayer(agroTriggerArea))
                {
                    states = CombatStatesKaiju.Agro;
                    _agent.speed = agroSpeed;
                }
                else
                {
                    if (FindPlayer(awaitTriggerArea))
                    {
                        if (states != CombatStatesKaiju.Await)
                        {
                            if (_canSwitch)
                            {
                                states = CombatStatesKaiju.Await;
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
                        if (states != CombatStatesKaiju.Normal)
                        {
                            if (_canSwitch)
                            {
                                states = CombatStatesKaiju.Normal;
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
            if (states == CombatStatesKaiju.Await)
            {
                _agent.destination = _target.transform.position;
                _agent.stoppingDistance = awaitPlayerDistance;

                _canSwitch = false;
            }

            else if (states == CombatStatesKaiju.Normal)
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
                case CombatStatesKaiju.Agro: Agro(); break;
                case CombatStatesKaiju.Await:
                    LookTarget();
                    AIMove();
                    break;
                case CombatStatesKaiju.Normal: AIMove(); break;
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

        public void BackOff(Vector3 p_pos, float p_stopping = 0.2f)
        {
            MoveTo(p_pos, p_stopping);
            LookTarget();
        }

        public void MoveTo(Vector3 p_pos, float p_stopping = 0.2f)
        {
            _agent.destination = p_pos;
            _agent.stoppingDistance = p_stopping;
        }

        public void LookTarget()
        {
            Vector3 t_direction = _target.transform.position - transform.position;
            t_direction.y = 0; // On ignore la composante Y pour éviter l'inclinaison

            // Vérifier que la direction n'est pas nulle pour éviter des erreurs
            if (t_direction != Vector3.zero)
            {
                Quaternion t_targetRotation = Quaternion.LookRotation(t_direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, t_targetRotation, _agent.angularSpeed * Time.deltaTime);
            }
        }

        public Vector3 GetPositionBehind(float p_distance)
        {
            // On prend la direction vers la target et on l'inverse
            Vector3 direction = (transform.position - _target.transform.position).normalized;
            return transform.position + direction * p_distance;
        }

        public void Attack(int p_damage, Vector3 attackCenter, Vector3 attackSize, Effect p_effect = null, float p_effectTime = 0)
        {
            Collider[] t_collisions = Physics.OverlapBox(
                transform.position + transform.rotation * attackCenter, // Appliquer la rotation à l'offset
                attackSize / 2,                                         // Demi-taille de la boîte
                transform.rotation,                                     // Rotation de la boîte
                LayerMask.GetMask("Player")                             // Layer ciblé
            );
            if (t_collisions.Length > 0)
            {
                _dps += p_damage;
                textDPS.text = _dps.ToString();
                if(p_effect != null)
                {
                    MechaInstance t_mecha = t_collisions[0].GetComponent<MechaInstance>();
                    t_mecha.AddEffect(p_effect, p_effectTime);
                }
            }

        }

        protected void OnDrawGizmos()
        {
            if(!showGizmo) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, agroTriggerArea);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, awaitTriggerArea);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, awaitPlayerDistance);
        }

        IEnumerator ShowDPS()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                _dps = 0;
                textDPS.text = _dps.ToString();
            }
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
