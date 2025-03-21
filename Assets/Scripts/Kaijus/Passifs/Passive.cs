namespace Mekaiju.AI.Passive
{
    [System.Serializable]
    public abstract class Passive
    {
        public float cooldown;
        public float launchTime;

        public bool isUsed { get; protected set; }

        protected bool _using = true;

        protected bool _canUse = true;

        public Passive()
        {
            isUsed = true;
            _using = true;
        }

        public virtual void Active()
        {
            isUsed = true;
            _using = true;
        }

        public virtual void Run(KaijuInstance p_kaiju) { }

        public virtual void OnDamage() { }

        public virtual void OnStart()
        {
            _canUse = true;
            _using = false;
            isUsed = false;
        }
    }
}
