using MyBox;
using UnityEngine;
using System;

namespace Mekaiju.AI
{
    [Serializable]
    public abstract class KaijuBehavior
    {
        public float speed = 3;
        public bool canTrigger = false;
        [ConditionalField(nameof(canTrigger))] public float triggerArea = 30f;
        [ConditionalField(nameof(canTrigger))] public bool triggerOnce = false;
        [ConditionalField(nameof(canTrigger))] public bool showGizmo = false;

        protected GameObject _target;
        protected GameObject _kaiju;

        protected KaijuInstance _kaijuInstance;

        public bool active { get { return _active; } }
        private bool _active;

        public void Init(GameObject p_target, GameObject p_kaiju)
        {
            _active = false;
            _target = p_target;
            _kaiju = p_kaiju;

            _kaijuInstance = _kaiju.GetComponent<KaijuInstance>();
        }

        public void Trigger()
        {
            if(!_kaijuInstance.canSwitch()) return;
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
