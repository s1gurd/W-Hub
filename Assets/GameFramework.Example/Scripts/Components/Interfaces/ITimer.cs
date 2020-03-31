namespace GameFramework.Example.Components.Interfaces
{
    public interface ITimer
    {
        void FinishTimer();
        void StartTimer();
        bool TimerActive { get; set; }
        TimerComponent Timer { get; }
    }
}