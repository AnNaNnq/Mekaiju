using Mekaiju.Attribute;
using MyBox;
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

        [Foldout("Debug")]
        [OverrideLabel("Show Gizmo For Face Attack")]
        public bool debugAttak1 = false;
        [OverrideLabel("Color For Face Attack Range")]
        [ConditionalField(nameof(debugAttak1))] public Color debugAttack1RangeColor = Color.red;


        public override void Agro()
        {
            base.Agro();
            
        }

        protected void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (debugAttak1)
            {
                Gizmos.color = debugAttack1RangeColor;
                Gizmos.DrawWireSphere(transform.position, attack1Range);
            }
        }
    }
}
