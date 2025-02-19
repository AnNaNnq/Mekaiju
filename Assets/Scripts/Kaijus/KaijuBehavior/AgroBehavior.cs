using UnityEngine;

namespace Mekaiju.AI
{
    public class AgroBehavior : KaijuBehavior
    {
        public override void Run()
        {
            base.Run();
            _kaijuInstance.Combat();
        }
    }
}
