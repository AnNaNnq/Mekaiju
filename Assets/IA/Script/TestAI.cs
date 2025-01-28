using UnityEngine;

namespace Mekaiju.AI
{
    public class TestAI : BasicAI
    {
        public override void Agro()
        {
            base.Agro();

            //_agent.destination = _target;
        }

    }
}
