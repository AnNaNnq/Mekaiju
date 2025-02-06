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
        [HideInInspector][PositiveValueOnly][OverrideLabel("Attack Countdown (sec)")] public float attackCountdown = 0.2f;
        
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

        [Foldout("Debug")]
        [OverrideLabel("Show Gizmo For Non-agro Phase")]
        public bool showGizmo;
        public TextMeshProUGUI textDPS;


        private int _dps = 0;

        /// <summary>
        /// Initialisation des variables importantes
        /// </summary>
        protected void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            states = CombatStatesKaiju.Normal;
            _target = GameObject.FindGameObjectWithTag(targetTag);
            StartCoroutine(ShowDPS());
            textDPS.text = _dps.ToString();
        }

        /// <summary>
        /// Activation des machines à états à chqaue frame
        /// </summary>
        protected void Update()
        {
            ChangeState();
            CombatStatesMachine();
        }

        /// <summary>
        /// Changement d'état
        /// </summary>
        public void ChangeState()
        {
            // Si l'ennemi n'est pas en mode agro on cherche à changer d'état (s'il est en agro il reste en agro)
            if (states != CombatStatesKaiju.Agro)
            {
                // Si le joueur est dans la zone d'agro, on passe en mode agro
                if (FindPlayer(agroTriggerArea))
                {
                    states = CombatStatesKaiju.Agro;
                    _agent.speed = agroSpeed;
                }
                else
                {
                    // Si le joueur est dans la zone d'attente, on passe en mode attente
                    if (FindPlayer(awaitTriggerArea))
                    {
                        if (states != CombatStatesKaiju.Await)
                        {
                            if (_canSwitch)
                            {
                                // On change d'état et de vitesse
                                states = CombatStatesKaiju.Await;
                                _agent.speed = awaitSpeed;
                            }
                            else
                            {
                                // On a un temps de latence avant de passer en mode attente
                                StartCoroutine(Countdown(timeBeforeAwait));
                            }
                        }
                    }
                    else
                    {
                        // Si le joueur n'est pas dans la zone d'attente, on passe en mode normal
                        if (states != CombatStatesKaiju.Normal)
                        {
                            if (_canSwitch)
                            {
                                // On change d'état et de vitesse
                                states = CombatStatesKaiju.Normal;
                                _agent.speed = normalSpeed;
                            }
                            else
                            {
                                // On a un temps de latence avant de passer en mode normal
                                StartCoroutine(Countdown(timeBeforeNormal));
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Pour déplacer le Kaiju
        /// </summary>
        public void AIMove()
        {
            // Si on est en attente, on se dirige vers le joueur, tout en restant a distance
            if (states == CombatStatesKaiju.Await)
            {
                _agent.destination = _target.transform.position;
                _agent.stoppingDistance = awaitPlayerDistance;

                _canSwitch = false;
            }
            // Si on est en mode normal, on se dirige vers le nid
            else if (states == CombatStatesKaiju.Normal)
            {
                _agent.SetDestination(nest.position);
                _agent.stoppingDistance = 0.3f;
                _canSwitch = false;
            }
        }

        /// <summary>
        /// La machine a état
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
        /// Permet de voir si le joueur est à une certaine range
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
        /// Fait reculer le Kaiju tout en regardant le joueur
        /// </summary>
        /// <param name="p_pos"></param>
        /// <param name="p_stopping"></param>
        public void BackOff(Vector3 p_pos, float p_stopping = 0.2f)
        {
            MoveTo(p_pos, p_stopping);
            LookTarget();
        }


        /// <summary>
        /// Fait avancer le Kaiju vers une position donnée
        /// </summary>
        /// <param name="p_pos"></param>
        /// <param name="p_stopping"></param>
        public void MoveTo(Vector3 p_pos, float p_stopping = 0.2f)
        {
            _agent.destination = p_pos;
            _agent.stoppingDistance = p_stopping;
        }


        /// <summary>
        /// Force le Kaiju a regarder le joueur
        /// </summary>
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

        /// <summary>
        /// Récupérer la position derrière le Kaiju
        /// </summary>
        /// <param name="p_distance"></param>
        /// <returns></returns>
        public Vector3 GetPositionBehind(float p_distance)
        {
            // On prend la direction vers la target et on l'inverse
            Vector3 direction = (transform.position - _target.transform.position).normalized;
            return transform.position + direction * p_distance;
        }


        /// <summary>
        /// Attaque le joueur
        /// </summary>
        /// <param name="p_damage"></param>
        /// <param name="attackCenter"></param>
        /// <param name="attackSize"></param>
        /// <param name="p_effect"></param>
        /// <param name="p_effectTime"></param>
        public void Attack(int p_damage, Vector3 attackCenter, Vector3 attackSize, Effect p_effect = null, float p_effectTime = 0)
        {
            // On vérifie si on peut attaquer
            Collider[] t_collisions = Physics.OverlapBox(
                transform.position + transform.rotation * attackCenter, // Appliquer la rotation à l'offset
                attackSize / 2,                                         // Demi-taille de la boîte
                transform.rotation,                                     // Rotation de la boîte
                LayerMask.GetMask("Player")                             // Layer ciblé
            );
            // Si on a touché le joueur
            if (t_collisions.Length > 0)
            {
                _dps += p_damage;
                textDPS.text = _dps.ToString();
                //Applique un effet s'il y en a
                if(p_effect != null)
                {
                    MechaInstance t_mecha = t_collisions[0].GetComponent<MechaInstance>();
                    t_mecha.AddEffect(p_effect, p_effectTime);
                }
            }

        }

        public void TakeDamage(int p_damage, GameObject p_tuchObject)
        {
            BodyPart t_bodyPart = GetBodyPartWithGameObject(p_tuchObject);
            t_bodyPart.health -= p_damage;
            if(t_bodyPart.health <= 0)
            {
                t_bodyPart.isDestroyed = true;
                t_bodyPart.health = 0;
            }
        }

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
        /// Fonction virtuel pour le combat
        /// </summary>
        public virtual void Agro() {}

        /// <summary>
        /// 
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
        /// 
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
