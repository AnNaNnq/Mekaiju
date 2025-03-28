namespace Mekaiju.Tuto
{
    public interface ITutorial
    {
        public abstract void Execute(MechaInstance p_mecha, TutorialInstance p_instance);

        public abstract void End(MechaInstance p_mecha);
    }
}
