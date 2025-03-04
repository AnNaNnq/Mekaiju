using MyBox;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class RockLook : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        [OverrideLabel("Look Duration (sec)")]
        public float lookDuration = 2f;

        MeshRenderer _rend;

        Transform _start;

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            _start = GameObject.FindGameObjectWithTag("DoomsdayRaySpawn").transform;
            kaiju.motor.StopKaiju(lookDuration);

            _rend = _start.GetComponent<MeshRenderer>();
            kaiju.StartCoroutine(Attack(kaiju));
        }

        public override IEnumerator Attack(KaijuInstance kaiju)
        {
            base.Attack(kaiju);
            bool t_stop = false;
            float t_time = 0;
            while (!t_stop)
            {
                yield return new WaitForSeconds(.1f);
                Debug.Log(_rend.isVisible);
            }
        }
    }
}
