namespace Mekaiju.Tuto
{
    public interface ITutorial
    {
        public abstract void Execute(MechaInstance p_mecha);

        public abstract void End(MechaInstance p_mecha);
    }
}
