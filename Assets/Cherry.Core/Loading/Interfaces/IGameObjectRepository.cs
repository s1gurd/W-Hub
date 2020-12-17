namespace GameFramework.Example.Loading.Interfaces
{
    public interface IGameObjectRepository
    {
        T Get<T>(string name) where T : UnityEngine.Object;
    }
}