using UnityEngine;

namespace Mekaiju.Utils
{
    public static class GameObjectExtension
    {
        public static MechaPartInstance GetMechaPartInstance(this GameObject p_go)
        {
            if (p_go.TryGetComponent<MechaPartInstance>(out var t_inst))
            {
                return t_inst;
            }
            else
            {
                if (p_go.TryGetComponent<MechaPartGetterProxy>(out var t_mpgp))
                {
                    return t_mpgp.instance;
                }
            }
            return null;
        }

        public static bool TryGetMechaPartInstance(this GameObject p_go, out MechaPartInstance p_inst)
        {
            p_inst = null;
            if (p_go.TryGetComponent<MechaPartInstance>(out p_inst))
            {
                return true;
            }

            if (p_go.TryGetComponent<MechaPartGetterProxy>(out var t_mpgp))
            {
                p_inst = t_mpgp.instance;
                if (p_inst)
                {
                    return true;
                }
                else
                {
                    Debug.Log($"[{p_go.name}] MechaPartGetterProxy.instance is null.");
                }
            }

            return false;
        }
    }
}
