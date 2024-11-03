using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public SubScene subScene;

    private World world;
    private EntityManager entityManager;

    private void Awake()
    {
        if (subScene == null)
        {
            Debug.LogError("sub Scene is null");
            return;
        }

        world = World.DefaultGameObjectInjectionWorld;
        entityManager = world.EntityManager;

        SceneSystem.LoadSceneAsync(world.Unmanaged, subScene.SceneGUID, new SceneSystem.LoadParameters { AutoLoad = true });
    }
    private void OnDestroy()
    {

    }
}
