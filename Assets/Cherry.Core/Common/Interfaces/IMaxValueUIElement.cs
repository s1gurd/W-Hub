namespace GameFramework.Example.Common.Interfaces
{
    public interface IMaxValueUIElement : IUIElement
    {
        string MaxValueAssociatedID { get; }

        void SetMaxValue(object maxValue);
    }
}