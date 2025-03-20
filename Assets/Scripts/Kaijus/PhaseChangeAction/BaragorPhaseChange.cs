using Mekaiju.Entity.Effect;
using Mekaiju.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Mekaiju.AI.PhaseAttack
{
    public class BaragorPhaseChange : PhaseAttack
    {
        List<Vector3> _pos;
        public LayerMask mask;
        public int maxBounce = 5;

        public override void Action()
        {
            base.Action();

            UtilsFunctions.CastLaser(_kaiju.transform.position, _kaiju.transform.forward, _pos, _kaiju.transform, maxBounce, mask, _kaiju.GetComponent<LineRenderer>());
        }
    }
}
