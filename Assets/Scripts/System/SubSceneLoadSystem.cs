using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;

partial struct SubSceneLoadSystem : ISystem
{
    private EntityQuery resolvedSectionEntityQuery;
    private EntityQuery entityPrefabComponentQuery;
    void OnCreate(ref SystemState state)
    {
        state.EntityManager.CreateSingleton<PrefabMapComponent>();
        state.RequireForUpdate<ResolvedSectionEntity>();
        resolvedSectionEntityQuery = state.GetEntityQuery(typeof(ResolvedSectionEntity));
        entityPrefabComponentQuery = state.GetEntityQuery(typeof(EntityPrefabComponent));
    }

    void OnDestroy(ref SystemState state) 
    {
        var component = SystemAPI.GetSingleton<PrefabMapComponent>();
        if (!component.dict.IsEmpty)
        {
            component.dict.Dispose();
        }
    }

    void OnUpdate(ref SystemState state)
    {
        if (resolvedSectionEntityQuery.CalculateEntityCount() <= 0)
            return;

        var subSceneCompleteTagLookup = SystemAPI.GetComponentLookup<SubSceneCompleteTag>(true);
        var Entities = resolvedSectionEntityQuery.ToEntityArray(Allocator.Temp);
        bool isUpdate = false;
        int completeCount = 0;
        foreach (var sceneEntity in Entities)
        {
            if (subSceneCompleteTagLookup.HasComponent(sceneEntity))
            {
                completeCount++;
                continue;
            }

            if (SceneSystem.IsSceneLoaded(state.WorldUnmanaged, sceneEntity))
            {
                state.EntityManager.AddComponent<SubSceneCompleteTag>(sceneEntity);
                isUpdate = true;
                completeCount++;
            }
        }

        bool isAllLoaded = completeCount == Entities.Length;
        Entities.Dispose();
        if (isUpdate)
        {
            var components = entityPrefabComponentQuery.ToComponentDataArray<EntityPrefabComponent>(Allocator.Temp);
            if(SystemAPI.TryGetSingleton<PrefabMapComponent>(out var prefabComponent))
            {
                if (!prefabComponent.dict.IsEmpty)
                {
                    prefabComponent.dict.Dispose();
                }
            }
            prefabComponent = new PrefabMapComponent();
            prefabComponent.dict = new NativeParallelHashMap<int, Entity>(components.Length, Allocator.Persistent);
            foreach (var component in components)
            {
                prefabComponent.dict[component.index] = component.Entity;
            }
            SystemAPI.SetSingleton(prefabComponent);
            components.Dispose();
        }

        if (isAllLoaded)
        {
            EventSystem.Publish(default(GameStart));
            state.Enabled = false;
        }
    }
}
