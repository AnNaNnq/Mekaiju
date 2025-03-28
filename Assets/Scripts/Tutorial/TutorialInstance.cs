using System.Collections.Generic;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.AI.Objet;
using Mekaiju.Entity;
using Mekaiju.Entity.Effect;
using Mekaiju.Utils;
using MyBox;
using UnityEngine;
using UnityEngine.AI;

namespace Mekaiju.Tuto
{
    public class TutorialInstance : EntityInstance
    {
        [Separator("General")]
        [Tag] public string targetTag;
        public KaijuStats stats;
        public BodyPart[] bodyParts;

        [Separator("Tutorial Phase")]
        public TutorialPhase phaseOne;

        private int _turorialPhase = 0;

        private MechaInstance _mecha;
        private GameObject _mechaObject;

        private List<TutoStep> _steps;

        private NavMeshAgent _agent;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _steps = UtilsFunctions.LoadAllSO<TutoStep>();
            _mechaObject = GameObject.FindGameObjectWithTag(targetTag);
            _mecha = _mechaObject.GetComponent<MechaInstance>();
            _turorialPhase = 0;
            tutorialExecutoin();
        }

        public void tutorialExecutoin()
        {
            if(_steps.Count > _turorialPhase) _steps[_turorialPhase].behavior.Execute(_mecha);
        }

        public bool IsInMovement()
        {
            return _agent.velocity.magnitude > 0.1f && !_agent.isStopped;
        }

        #region implemation of IEntityInstance
        public override bool isAlive => throw new System.NotImplementedException();

        public override float health => throw new System.NotImplementedException();

        public override float baseHealth => throw new System.NotImplementedException();

        public override float Heal(float p_amount)
        {
            throw new System.NotImplementedException();
        }

        public override float TakeDamage(IDamageable p_from, float p_damage, DamageKind p_kind)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }


    [System.Serializable]
    public class TutorialPhase
    {
        public Effect stunEffect;
        public KaijuAttack kaijuAttack;
    }
}
