using System.Collections;
using Mekaiju;
using Mekaiju.AI.Body;

public class EmptyAbility : IAbilityBehaviour
{
    public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
    {
        return false;
    }

    public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
    {
        yield return null;
    }
}
