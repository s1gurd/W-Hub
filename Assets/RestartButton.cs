using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void Restart()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entityArray = entityManager.GetAllEntities();
        foreach (var e in entityArray)
            entityManager.DestroyEntity(e);
        entityArray.Dispose();
        SceneManager.LoadScene(1);
    }
}
