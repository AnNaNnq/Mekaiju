namespace Mekaiju.AI.Behavior
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
