namespace Mekaiju.AI.Passive
{
    [System.Serializable]
    public abstract class IPassive
    {
        public float cooldown;
        public float launchTime;

        public bool isUsed { get; protected set; }

        protected bool _using = true;

        protected bool _canUse = true;

        public IPassive()
        {
            isUsed = true;
            _using = true;
        }

        public virtual void Active()
        {
            isUsed = true;
            _using = true;
        }

        public virtual void Passive(KaijuInstance kaiju) { }

        public virtual void OnDamage() { }

        public virtual void OnStart()
        {
            _canUse = true;
            _using = false;
            isUsed = false;
        }
    }
}
