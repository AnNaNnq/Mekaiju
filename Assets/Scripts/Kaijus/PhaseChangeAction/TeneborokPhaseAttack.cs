using Mekaiju.Attribute;
using Mekaiju.Entity.Effect;
using MyBox;
using System;
using UnityEngine;
namespace Mekaiju.AI.PhaseAttack
{
    public class TeneborokPhaseAttack : PhaseAttack
    {
        [Separator]
        [SOSelector]
        public Effect effect;
        [Header("Fail")]
        public int healAmountWhenFail = 50;

        IDisposable _effectUsed;

        public override void Action()
        {
            base.Action();
            _kaiju.motor.StopKaiju();
            MechaInstance t_mecha = _kaiju.target.GetComponent<MechaInstance>();
            _effectUsed = t_mecha.AddEffect(effect);

            input = UnityEngine.Random.Range(0, _qte.qteInputActions.Count);

            _qte.StartQTE();
        }

        public override void Success()
        {
            base.Success();
            MechaInstance t_mecha = _kaiju.target.GetComponent<MechaInstance>();
            t_mecha.RemoveEffect(_effectUsed);
            _kaiju.motor.StartKaiju();
            _kaiju.SetPhase(2);
        }

        public override void Failure()
        {
            base.Failure();
            MechaInstance t_mecha = _kaiju.target.GetComponent<MechaInstance>();
            t_mecha.RemoveEffect(_effectUsed);

            var t_damage = _kaiju.GetRealDamage(damage);

            t_mecha.TakeDamage(t_damage);
            _kaiju.motor.StartKaiju();
            _kaiju.Heal(healAmountWhenFail);
            _kaiju.SetPhase(1);
        }
    }
}
