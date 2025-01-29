using Mekaiju.Attribute;
using MyBox;
using UnityEditor;
using UnityEngine;

namespace Mekaiju.AI
{
    public class TestAI : BasicAI
    {
        [Separator("Combat Stat")]
        [SelectFromList(nameof(bodyParts))]
        public int test;

        public override void Agro()
        {
            base.Agro();

            //_agent.destination = _target;
        }

    }
}
