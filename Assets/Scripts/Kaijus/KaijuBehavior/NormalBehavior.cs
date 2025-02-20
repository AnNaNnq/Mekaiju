using Mekaiju.Attribute;
using UnityEngine;

namespace Mekaiju.AI
{
    public class NormalBehavior : KaijuBehavior
    {
        [FocusObject]
        public Transform nest;

        public override void Run()
        {
            base.Run();
            _kaijuInstance.motor.MoveTo(nest.position, speed);
        }
    }
}
