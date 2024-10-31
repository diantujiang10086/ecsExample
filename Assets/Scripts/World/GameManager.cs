using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SubScene subScene;

    private void Awake()
    {
        if (subScene == null)
        {
            Debug.LogError("sub Scene is null");
            return;
        }

        GlobalData.Initialize();

        var world = World.DefaultGameObjectInjectionWorld;
        SceneSystem.LoadSceneAsync(world.Unmanaged, subScene.SceneGUID, new SceneSystem.LoadParameters { AutoLoad = true });
    }
    private void OnDestroy()
    {
        GlobalData.Clear();
    }
}
