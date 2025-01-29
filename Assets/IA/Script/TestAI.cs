using MyBox;
using UnityEngine;

namespace Mekaiju.AI
{
    public class TestAI : BasicAI
    {
        [Separator("Combat Stat")]
        public float test;

        public override void Agro()
        {
            base.Agro();

            //_agent.destination = _target;
        }

    }
}
