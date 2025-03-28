using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju
{
    public class MechaPartCollisionProxy : MechaPartProxy
    {
        public UnityEvent<Collider> onCollide = new();

        public MechaPartInstance instance;

        public void Setup(MechaPartInstance p_inst)
        {
            instance = p_inst;
            onCollide.AddListener(t_collider => p_inst.onCollide.Invoke(t_collider));
        }

        private void OnTriggerEnter(Collider p_collider)
        {
            onCollide.Invoke(p_collider);
        }
    }
}