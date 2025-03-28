using Mekaiju;
using UnityEngine;

public class MechaPartGetterProxy : MechaPartProxy
{
    [field: SerializeField]
    public MechaPartInstance instance { get; private set; }

    public void Setup(MechaPartInstance p_inst)
    {
        instance = p_inst;
    }
}
