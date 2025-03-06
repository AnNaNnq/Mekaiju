using System.Collections;
using Mekaiju;
using Mekaiju.AI.Body;
using Mekaiju.Entity;

public class EmptyAbility : IAbilityBehaviour
{
    public override bool IsAvailable(EntityInstance p_self, object p_opt)
    {
        return false;
    }

    public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
    {
        yield return null;
    }
}
