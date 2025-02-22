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
    public class KaijuInstance : IEntityInstance
    {
        [Header("General")]
        [Tag] public string targetTag;
        public KaijuStats stats;

        [Separator]
        [field: SerializeReference, SubclassPicker]
        public List<KaijuBehavior> behaviors = new List<KaijuBehavior>();
        public float timeBetweenTowAction = 1f;

        [SerializeField]
        private bool _canBehaviorSwitch = true;

        public GameObject target { get; private set; }

        [HideInInspector]
        public KaijuMotor motor { get { return _motor; } }

        protected KaijuMotor _motor;

        private KaijuBrain _brain;

        [SOSelector]
        public KaijuAttackContainer attackGraph;

        [Separator]
        [Header("Debug")]
        public bool checkRange;
        [ConditionalField(nameof(checkRange))] public float debugRange ;

        private void Start()
        {
            _motor = GetComponent<KaijuMotor>();
            _brain = GetComponent<KaijuBrain>();
            target = GameObject.FindGameObjectWithTag(targetTag);
            foreach (var behavior in behaviors)
            {
                behavior.Init(target, gameObject);
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

        public bool TargetInRange(float p_range)
        {
            return Vector3.Distance(target.transform.position, transform.position) <= p_range;
        }

        private void OnDrawGizmos()
        {
            foreach (var behavior in behaviors.Where(b => b.showGizmo))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, behavior.triggerArea);
            }

            if (checkRange)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, debugRange);
            }
        }

#region IEntityInstance implemetation

        // TODO: implement
        public override EnumArray<ModifierTarget, ModifierCollection> modifiers => throw new System.NotImplementedException();

        public override float baseHealth => throw new System.NotImplementedException();

        public override void Heal(float p_amount)
        {
            throw new System.NotImplementedException();
        }

        public override void TakeDamage(float p_damage)
        {
            throw new System.NotImplementedException();
        }
#endregion
    }
}
