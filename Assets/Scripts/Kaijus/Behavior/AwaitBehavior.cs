namespace Mekaiju.AI.Behavior
{
    public class AwaitBehavior : KaijuBehavior
    {
        public float playerDistance = 20f;

        public override void Run()
        {
            base.Run();
            _kaijuInstance.motor.SetSpeed(speed);
            _kaijuInstance.motor.MoveTo(_target.transform.position, playerDistance);
        }
    }
}
