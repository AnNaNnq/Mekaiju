using Mekaiju.Utils;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class ParasiticRash : IAttack
    {
        [Separator]
        public int nbParasites = 3;
        public GameObject parasitePrefab;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            for (int i = 0; i < nbParasites; i++)
            {
                Vector3 p_pos = UtilsFunctions.GetRandomPointInCircle(range, kaiju.transform);
                p_pos = new Vector3(p_pos.x, UtilsFunctions.GetGround(p_pos), p_pos.z);
                GameObject t_parasite = GameObject.Instantiate(parasitePrefab, p_pos, Quaternion.identity);
            }
        }

    }
}
