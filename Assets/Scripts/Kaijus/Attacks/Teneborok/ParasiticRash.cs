using Mekaiju.Entity;
using Mekaiju.Utils;
using MyBox;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class ParasiticRash : Attack
    {
        [Separator]
        public int nbParasites = 3;
        public GameObject parasitePrefab;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);
            for (int i = 0; i < nbParasites; i++)
            {
                Vector3 t_pos = UtilsFunctions.GetRandomPointInCircle(range, p_kaiju.transform);
                t_pos = new Vector3(t_pos.x, UtilsFunctions.GetGround(t_pos), t_pos.z);
                GameObject t_parasite = GameObject.Instantiate(parasitePrefab, t_pos, Quaternion.identity);
                OnEnd();
            }
        }

    }
}
