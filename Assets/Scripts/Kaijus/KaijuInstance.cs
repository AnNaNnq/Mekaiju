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

        private void Start()
        {
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
    }
}
