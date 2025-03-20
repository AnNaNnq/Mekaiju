using Mekaiju.AI.Attacl;
using Mekaiju.Entity;
using TMPro;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class GroundStrike : Attack, IShockWave
    {
        public ShockWaveStat wave;

        Transform _pos;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);
            _kaiju.animator.AttackAnimation(nameof(GroundStrike));
            _pos = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;
            wave.kaiju = _kaiju;
        }

        public override void OnAction()
        {
            base.OnAction();
            IShockWave t_shock = this;
            t_shock.LunchWave(wave, _pos);
        }
    }
}
