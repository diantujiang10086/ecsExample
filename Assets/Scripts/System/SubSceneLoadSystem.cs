using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;

partial struct SubSceneLoadSystem : ISystem
{
    private EntityQuery resolvedSectionEntityQuery;
    private EntityQuery entityPrefabComponentQuery;
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResolvedSectionEntity>();
        resolvedSectionEntityQuery = state.GetEntityQuery(typeof(ResolvedSectionEntity));
        entityPrefabComponentQuery = state.GetEntityQuery(typeof(EntityPrefabComponent));
    }

    void OnUpdate(ref SystemState state)
    {
        if (resolvedSectionEntityQuery.CalculateEntityCount() <= 0)
            return;

        var subSceneCompleteTagLookup = SystemAPI.GetComponentLookup<SubSceneCompleteTag>(true);
        var Entities = resolvedSectionEntityQuery.ToEntityArray(Allocator.Temp);
        bool isUpdate = false;
        foreach (var sceneEntity in Entities)
        {
            if (subSceneCompleteTagLookup.HasComponent(sceneEntity))
                continue;

            if (SceneSystem.IsSceneLoaded(state.WorldUnmanaged, sceneEntity))
            {
                state.EntityManager.AddComponent<SubSceneCompleteTag>(sceneEntity);
                isUpdate = true;
            }
        }
        Entities.Dispose();
        if (isUpdate)
        {
            var components = entityPrefabComponentQuery.ToComponentDataArray<EntityPrefabComponent>(Allocator.Temp);
            GlobalData.UpdatePrefab(components);
            components.Dispose();
        }
    }
}
