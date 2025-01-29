using Mekaiju.Attribute;
using MyBox;
using System.ComponentModel;

namespace Mekaiju.AI
{
    public class TestAI : BasicAI
    {
        [Foldout("Attaque de face")]
        [OverrideLabel("Degat")]
        public int attack1dmg = 5;


        public override void Agro()
        {
            base.Agro();

            //_agent.destination = _target;
        }

    }
}
