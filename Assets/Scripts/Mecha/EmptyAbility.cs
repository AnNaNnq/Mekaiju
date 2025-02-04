using System.Collections;
using Mekaiju;
using Mekaiju.AI;

public class EmptyAbility : IAbilityBehaviour
{
    public override IEnumerator Trigger(MechaInstance p_self, BasicAI p_target, object p_opt)
    {
        yield return null;
    }

    public override float Consumption(object p_opt)
    {
        return 0f;
    }
}
