using Mekaiju.Attributes;
using UnityEngine;
using System.Collections.Generic;
using MyBox;
using System.Linq;
using Mekaiju.Attribute;
using Mekaiju.Utils;

namespace Mekaiju.AI
{
    [RequireComponent(typeof(KaijuBrain))]
    [RequireComponent(typeof(KaijuMotor))]
    public class KaijuInstance : MonoBehaviour
    {
        [Header("General")]
        [Tag] public string targetTag;
        public KaijuStats stats;

        [Separator]
        [field: SerializeReference, SubclassPicker]
        public List<KaijuBehavior> behaviors = new List<KaijuBehavior>();

        [SerializeField]
        private bool _canBehaviorSwitch = true;

        private GameObject _target;

        [HideInInspector]
        public KaijuMotor motor { get { return _motor; } }
        protected KaijuMotor _motor;

        private KaijuBrain _brain;

        [SOSelector]
        public KaijuAttackContainer attackGraph;

        private void Start()
        {
            _motor = GetComponent<KaijuMotor>();
            _brain = GetComponent<KaijuBrain>();
            _target = GameObject.FindGameObjectWithTag(targetTag);
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
                behavior.IsTrigger();
                if (behavior.active) behavior.Run();
            }
        }

        public bool canSwitch()
        {
            return behaviors.All(b => !b.active || !b.triggerOnce);
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

        public void Combat()
        {
            _brain.StarFight();
        }

        private void OnDrawGizmos()
        {
            foreach (var behavior in behaviors.Where(b => b.showGizmo))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, behavior.triggerArea);
            }
        }
    }
}
