using Mekaiju.AI.Attack.Instance;
using Mekaiju.AI.Attacl;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public interface IShockWave
    {

        public void LunchWave(ShockWaveStat p_stat, Transform p_start)
        {
            Vector3 t_pos = p_start.position;
            GameObject t_go = GameObject.Instantiate(p_stat.shockwavePrefab, t_pos, Quaternion.identity);
            ShockWaveObject t_sw = t_go.GetComponent<ShockWaveObject>();
            t_sw.SetUp(p_stat);
        }
    }
}