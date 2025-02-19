using MyBox;
using UnityEngine;
using System;

namespace Mekaiju.AI
{
    [Serializable]
    public abstract class KaijuBehavior
    {
        public float speed = 3;
        public float cooldown = 2;
        public bool canTrigger = false;
        [ConditionalField(nameof(canTrigger))] public float triggerArea = 30f;

        private GameObject _target;
        private GameObject _kaiju;

        private KaijuInstance _kaijuInstance;

        public bool active { get { return _active; } }
        [SerializeField]
        private bool _active;

        public void Init(GameObject p_target, GameObject p_kaiju)
        {
            _active = false;
            _target = p_target;
            _kaiju = p_kaiju;

            _kaijuInstance = _kaiju.GetComponent<KaijuInstance>();
        }

        protected void IsTrigger()
        {
            if(!canTrigger) _active = false;

            _active = GetDistance() <= triggerArea;

            _kaijuInstance.CheckAllBehaviorsDisabeled();
        }

        public virtual void Run() { }

        public void Active()
        {
            _active = true;
        }

        protected float GetDistance()
        {
            return Vector3.Distance(_target.transform.position, _kaiju.transform.position);
        }
    }
}
