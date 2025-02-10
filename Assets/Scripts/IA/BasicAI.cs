using Mekaiju.Attribute;
using MyBox;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Mekaiju.AI
{
    public abstract class BasicAI : MonoBehaviour
    {
        [Foldout("General")]
        public CombatStatesKaiju states;
        public LayerMask mask;
        [Tag] public string targetTag;
        public BodyPart[] bodyParts;
        [ReadOnly]
        public int nbPhase;

        [Foldout("Agro")]
        [PositiveValueOnly] public float agroTriggerArea = 10f;
        [PositiveValueOnly] public float agroSpeed = 3.5f;
        [PositiveValueOnly][OverrideLabel("Attack Countdown (sec)")] public float attackCountdown = 0.2f;
        [SerializeField]
        protected bool _canAttack = true;

        [Foldout("Await")]
        [PositiveValueOnly] public float awaitTriggerArea = 30f;
        [PositiveValueOnly] public float awaitPlayerDistance = 20f;
        [PositiveValueOnly] public float awaitSpeed = 3f;
        [PositiveValueOnly] [OverrideLabel("Time for switch to Await (sec)")] public float timeBeforeAwait = 2f;

        [Foldout("Normal")]
        [MustBeAssigned] [FocusObject] public Transform nest;
        [PositiveValueOnly] public float normalSpeed = 3f;
        [PositiveValueOnly] [OverrideLabel("Time for switch to Normal (sec)")] public float timeBeforeNormal = 2f;

        protected GameObject _target;

        protected NavMeshAgent _agent;

        private bool _canSwitch = false;

        protected int _currentPhase = 1;

        private int _totalStartHealth;

        [Foldout("Debug")]
        [OverrideLabel("Show Gizmo For Non-agro Phase")]
        public bool showGizmo;
        public TextMeshProUGUI textDPS;


        private int _dps = 0;

        /// <summary>
        /// Initializing important variables
        /// </summary>
        protected void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            states = CombatStatesKaiju.Normal;
            _target = GameObject.FindGameObjectWithTag(targetTag);
            StartCoroutine(ShowDPS());
            textDPS.text = _dps.ToString();
            _totalStartHealth = GetTotalHealth();
            _currentPhase = 1;

            // We add the BodyPartObject script to bodyParts objects if they don't already have it
            foreach (BodyPart t_part in bodyParts)
            {
                foreach (GameObject t_obj in t_part.part)
                {
                    if (t_obj.GetComponent<BodyPartObject>() == null)
                    {
                        t_obj.AddComponent<BodyPartObject>();
                    }
                }
            }
        }

        public int GetTotalHealth()
        {
            int t_health = 0;
            foreach (BodyPart t_part in bodyParts)
            {
                t_health += t_part.health;
            }
            return t_health;
        }

        /// <summary>
        /// Activation of frame state machines
        /// </summary>
        protected void Update()
        {
            ChangeState();
            CombatStatesMachine();
        }

        /// <summary>
        /// Change of state
        /// </summary>
        public void ChangeState()
        {
            // If the enemy is not in agro mode, we try to change its state (if it is in agro mode, it remains in agro mode).
            if (states != CombatStatesKaiju.Agro)
            {
                // If the player is in the agro zone, we switch to agro mode.
                if (FindPlayer(agroTriggerArea))
                {
                    states = CombatStatesKaiju.Agro;
                    _agent.speed = agroSpeed;
                }
                else
                {
                    // If the player is in the waiting area, we switch to waiting mode.
                    if (FindPlayer(awaitTriggerArea))
                    {
                        if (states != CombatStatesKaiju.Await)
                        {
                            if (_canSwitch)
                            {
                                // Changing state and speed
                                states = CombatStatesKaiju.Await;
                                _agent.speed = awaitSpeed;
                            }
                            else
                            {
                                // There is a latency time before switching to standby mode.
                                StartCoroutine(Countdown(timeBeforeAwait));
                            }
                        }
                    }
                    else
                    {
                        // If the player is not in the waiting area, we switch to normal mode.
                        if (states != CombatStatesKaiju.Normal)
                        {
                            if (_canSwitch)
                            {
                                // Changing state and speed
                                states = CombatStatesKaiju.Normal;
                                _agent.speed = normalSpeed;
                            }
                            else
                            {
                                // There is a delay before switching to normal mode
                                StartCoroutine(Countdown(timeBeforeNormal));
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// To move the Kaiju
        /// </summary>
        public void AIMove()
        {
            // If you are waiting, you move towards the player, but keep your distance.
            if (states == CombatStatesKaiju.Await)
            {
                _agent.destination = _target.transform.position;
                _agent.stoppingDistance = awaitPlayerDistance;

                _canSwitch = false;
            }
            // If in normal mode, we head for the nest
            else if (states == CombatStatesKaiju.Normal)
            {
                _agent.SetDestination(nest.position);
                _agent.stoppingDistance = 0.3f;
                _canSwitch = false;
            }
        }

        /// <summary>
        /// The state machine
        /// </summary>
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

        /// <summary>
        /// Shows whether the player is at a certain range
        /// </summary>
        /// <param name="p_range"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Makes the Kaiju move backwards while looking at the player
        /// </summary>
        /// <param name="p_pos"></param>
        /// <param name="p_stopping"></param>
        public void BackOff(Vector3 p_pos, float p_stopping = 0.2f)
        {
            MoveTo(p_pos, p_stopping);
            LookTarget();
        }


        /// <summary>
        /// Moves the Kaiju to a given position
        /// </summary>
        /// <param name="p_pos"></param>
        /// <param name="p_stopping"></param>
        public void MoveTo(Vector3 p_pos, float p_stopping = 0.2f)
        {
            _agent.destination = p_pos;
            _agent.stoppingDistance = p_stopping;
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


        /// <summary>
        /// Attack the player
        /// </summary>
        /// <param name="p_damage"></param>
        /// <param name="attackCenter"></param>
        /// <param name="attackSize"></param>
        /// <param name="p_effect"></param>
        /// <param name="p_effectTime"></param>
        public void Attack(int p_damage, Vector3 attackCenter, Vector3 attackSize, Effect p_effect = null, float p_effectTime = 0)
        {
            StartCoroutine(AttackCountdown());
            // We check if we can attack
            Collider[] t_collisions = Physics.OverlapBox(
                transform.position + transform.rotation * attackCenter, // Apply rotation to offset
                attackSize / 2,                                         // Half box size
                transform.rotation,                                     // Box rotation
                LayerMask.GetMask("Player")                             // Targeted layer
            );
            // If we hit the player
            if (t_collisions.Length > 0)
            {
                _dps += p_damage;
                textDPS.text = _dps.ToString();
                // Apply an effect if available
                if (p_effect != null)
                {
                    MechaInstance t_mecha = t_collisions[0].GetComponent<MechaInstance>();
                    t_mecha.AddEffect(p_effect, p_effectTime);
                }
            }
        }

        /// <summary>
        /// Makes the kaiju take damage on the right part of the body
        /// </summary>
        /// <param name="p_damage"></param>
        /// <param name="p_tuchObject"></param>
        public void TakeDamage(int p_damage, GameObject p_tuchObject)
        {
            if (states != CombatStatesKaiju.Agro) states = CombatStatesKaiju.Agro;

            BodyPart t_bodyPart = GetBodyPartWithGameObject(p_tuchObject);
            t_bodyPart.health -= p_damage;
            if(t_bodyPart.health <= 0)
            {
                t_bodyPart.isDestroyed = true;
                t_bodyPart.health = 0;
            }

            int t_curretHealth = GetTotalHealth();
            if (t_curretHealth <= (_totalStartHealth / 2))
            {
                _currentPhase++;
            }

            if (IsDead())
            {
                Debug.Log("Dead");
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Check if the Kaiju is dead
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            foreach (BodyPart t_part in bodyParts)
            {
                if (!t_part.isDestroyed)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get the body part with the GameObject
        /// </summary>
        /// <param name="p_object"></param>
        /// <returns></returns>
        public BodyPart GetBodyPartWithGameObject(GameObject p_object)
        {
            foreach (BodyPart t_part in bodyParts)
            {
                foreach (GameObject t_obj in t_part.part)
                {
                    if (t_obj == p_object)
                    {
                        return t_part;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Virtual combat function
        /// </summary>
        public virtual void Agro() {}

        /// <summary>
        /// Countdown function for switching states
        /// </summary>
        /// <param name="p_timer"></param>
        /// <returns></returns>
        public IEnumerator Countdown(float p_timer)
        {
            _canSwitch = false;
            yield return new WaitForSeconds(p_timer);
            _canSwitch = true;
        }

        /// <summary>
        /// Countdown function for attacking
        /// </summary>
        /// <returns></returns>
        public IEnumerator AttackCountdown()
        {
            yield return new WaitForSeconds(attackCountdown);
            _canAttack = true;
        }


        #region Fonction pour les LD

        /// <summary>
        /// Affiche les gizmos
        /// </summary>
        protected void OnDrawGizmos()
        {
            if (!showGizmo) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, agroTriggerArea);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, awaitTriggerArea);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, awaitPlayerDistance);
        }

        /// <summary>
        /// Affiche les DPS
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowDPS()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                _dps = 0;
                textDPS.text = _dps.ToString();
            }
        }

        #endregion
    }
}
