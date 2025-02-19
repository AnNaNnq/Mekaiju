using Mekaiju.Attributes;
using UnityEngine;
using System.Collections.Generic;
using MyBox;
using System.Linq;
using UnityEngine.AI;

namespace Mekaiju.AI
{
    public class KaijuInstance : MonoBehaviour
    {
        [Header("General")]
        [Tag] public string targetTag;

        [Separator]
        [field: SerializeReference, SubclassPicker]
        public List<KaijuBehavior> behaviors = new List<KaijuBehavior>();

        [Separator]
        [field: SerializeReference, SubclassPicker]
        public List<IAttack> attacks = new List<IAttack>();

        private GameObject _target;

        private NavMeshAgent _agent;

        private void Start()
        {
            _target = GameObject.FindGameObjectWithTag(targetTag);
            _agent = GetComponent<NavMeshAgent>();
            foreach (var behavior in behaviors)
            {
                behavior.Init(_target, gameObject);
            }

            CheckAllBehaviorsDisabeled();
        }


        private void Update()
        {
            UseBehavior();
        }

        public void UseBehavior()
        {
            foreach(var behavior in behaviors)
            {
                if (behavior.active) behavior.Run();
            }
        }

        public void CheckAllBehaviorsDisabeled()
        {
            if (behaviors.Count(b => !b.active) == behaviors.Count)
            {
                foreach (var behavior in behaviors.Where(b => !b.canTrigger))
                {
                    behavior.Active();
                }
            }
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
    }
}
