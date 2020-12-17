namespace GameFramework.Example.Common.Interfaces
{
    public interface IUIElement
    {
        string AssociatedID { get; }
        void SetData(object damage);
    }
}