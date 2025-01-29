using Mekaiju.AI.Attaque;
using Mekaiju.Attribute;
using MyBox;
using UnityEditor;
using UnityEngine;

namespace Mekaiju.AI
{
    public class TestAI : BasicAI
    {
        [Separator("Combat Stat")]
        public TypesAttaquesKaijuTuto type;
        [SerializeField, SerializeReference]
        public TutoAttaqueFace attaqueFace;

        public override void Agro()
        {
            base.Agro();
            attaqueFace.Execute();

            //_agent.destination = _target;
        }

        public void Start()
        {
            base.Start();
            attaqueFace= new TutoAttaqueFace();
        }
    }
}
