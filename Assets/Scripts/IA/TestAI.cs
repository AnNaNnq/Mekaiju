using Mekaiju.Attribute;
using MyBox;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Mekaiju.AI
{
    public class TestAI : BasicAI
    {
        [Foldout("Attaque de face")]
        [OverrideLabel("Damage")]
        public int attack1dmg = 5;
        [PositiveValueOnly]
        [OverrideLabel("Range")]
        public float attack1Range = 5;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int attack1Body;
        [OverrideLabel("Attack count")]
        public int attack1Count = 2;
        [OverrideLabel("Countdown (en s)")]
        public float attack1Coutdown = 2;

        private bool _canAttack1 = true;

        [Foldout("Charge")]
        [OverrideLabel("Damage")]
        public int attack2dmg = 5;
        [PositiveValueOnly]
        [OverrideLabel("Range max")]
        public float attack2RangeMax = 25;
        [OverrideLabel("Range min")]
        public float attack2RangeMin = 20;
        [OverrideLabel("Body Part")]
        [SelectFromList(nameof(bodyParts))] public int attack2Body;

        [Foldout("Debug")]
        [OverrideLabel("Show Gizmo For Face Attack")]
        public bool debugAttak1 = false;
        [ConditionalField(nameof(debugAttak1))] public Color colorForFaceAttackRange;
        [OverrideLabel("Show Gizmo For Range Attack")]
        public bool debugAttak2 = false;
        [ConditionalField(nameof(debugAttak2))] public Color colorForChargeMaxRange = Color.blue;
        [ConditionalField(nameof(debugAttak2))] public Color colorForChargeMinRange = Color.yellow;
        public TextMeshProUGUI textDPS;

        private int dps = 0;


        public override void Agro()
        {
            base.Agro();
            AttackStateMachine();
        }

        private new void Start()
        {
            base.Start();
            StartCoroutine(ShowDPS());
            textDPS.text = dps.ToString();
        }

        public void FaceAttack()
        {
            if (!_canAttack1) return;

            for (int i = 0; i < attack1Count; i++)
            {
                dps += attack1dmg;
                textDPS.text = dps.ToString();
            }
            StartCoroutine(Attack1Countdown());
        }


        public void AttackStateMachine()
        {
            float t_dist = Vector3.Distance(transform.position, _target.transform.position);
            if (t_dist < attack2RangeMin && t_dist > attack1Range)
            {
                MoveTo(_target.transform.position, attack1Range);
            }
            else if(t_dist <= attack1Range)
            {
                FaceAttack();
            }
        }

        protected new void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (debugAttak1)
            {
                Gizmos.color = colorForFaceAttackRange;
                Gizmos.DrawWireSphere(transform.position, attack1Range);
            }
            if (debugAttak2) {
                Gizmos.color = colorForChargeMaxRange;
                Gizmos.DrawWireSphere(transform.position, attack2RangeMax);
                Gizmos.color = colorForChargeMinRange;
                Gizmos.DrawWireSphere(transform.position, attack2RangeMin);
            }
        }

        IEnumerator Attack1Countdown()
        {
            _canAttack1 = false;
            yield return new WaitForSeconds(attack1Coutdown);
            _canAttack1 = true;
        }

        IEnumerator ShowDPS()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                dps = 0;
                textDPS.text = dps.ToString();
            }
        }
    }
}
