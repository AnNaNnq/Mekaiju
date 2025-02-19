using MyBox;
using NUnit.Framework.Internal;
using UnityEngine;

namespace Mekaiju.AI
{
    public class AwaitBehavior : KaijuBehavior
    {
        public float playerDistance = 20f;

        public override void Run()
        {
            base.Run();
            _motor.MoveTo(_target.transform.position, speed, playerDistance);
        }
    }
}
