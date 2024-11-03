using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct MapSystem : ISystem
{
    void OnCreate(ref SystemState state) 
    {
        state.RequireForUpdate<MapComponent>();
        state.RequireForUpdate<PrefabMapComponent>();
    }
    void OnUpdate(ref SystemState state) 
    {
        var prefabDict = SystemAPI.GetSingleton<PrefabMapComponent>().dict;
        if (prefabDict.IsEmpty)
            return;

        var entityManager = state.EntityManager;
        var mapComponent = SystemAPI.GetSingleton<MapComponent>();

        var mapRunComponentLookup = SystemAPI.GetComponentLookup<MapRuningComponent>();
        if (!mapRunComponentLookup.HasComponent(mapComponent.entity))
        {
            if(prefabDict.TryGetValue(mapComponent.playerId, out var prefab))
            {
                var player = entityManager.Instantiate(prefab);
                var localTransform = LocalTransform.FromPositionRotationScale(mapComponent.playerPosition, quaternion.identity, 1);
                entityManager.SetComponentData(player, localTransform);
                entityManager.AddComponent<PlayerComponent>(player);
            }
            entityManager.AddComponent<MapRuningComponent>(mapComponent.entity);
        }

        bool isCreate = false;
        var timeComponentLookup =  SystemAPI.GetComponentLookup<TimeComponent>(); 
        if(timeComponentLookup.TryGetComponent(mapComponent.entity, out var timeComponent))
        {
            timeComponent.Value -= SystemAPI.Time.DeltaTime;

            if(timeComponent.Value <= 0)
            {
                timeComponent.Value = mapComponent.spawnInterval;
                isCreate = true;
            }
            timeComponentLookup[mapComponent.entity] = timeComponent;
        }

        if (!isCreate)
            return;

        if (!prefabDict.TryGetValue(mapComponent.spawnEnemy, out var enemyEntity))
            return;
        var query = state.GetEntityQuery(typeof(PlayerComponent));
        var entities = query.ToEntityArray(Allocator.TempJob);
        var playerEntity = entities[0];
        
        var job = new SpawnJob
        {
            ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            playerPosition = entityManager.GetComponentData<LocalTransform>(playerEntity).Position,
            player = playerEntity,
            range = mapComponent.range,
            random = new Random((uint)UnityEngine.Random.Range(-99999999, 99999999)),
            prefabEntity = enemyEntity
        };
        var jobHandle = job.Schedule(mapComponent.spawnCount, 128);
        jobHandle.Complete();
        entities.Dispose();
    }

    struct SpawnJob : IJobParallelFor
    {
        [ReadOnly] public float3 playerPosition;
        [ReadOnly] public Entity player;
        [ReadOnly] public float range;
        [ReadOnly] public Entity prefabEntity;
        public Random random;
        public EntityCommandBuffer.ParallelWriter ecb;
        public void Execute(int index)
        {
            var entity = ecb.Instantiate(index, prefabEntity);
            var min = new float3(playerPosition.x - range, 0, playerPosition.z - range);
            var max = new float3(playerPosition.x + range, 0, playerPosition.z + range);
            var localTransform = LocalTransform.FromPositionRotationScale(random.NextFloat3(min, max), quaternion.identity, 1);
            ecb.SetComponent(index, entity, localTransform);
            ecb.SetComponent(index, entity, new LocalToWorld {Value = localTransform.ToMatrix() });
            ecb.SetComponent(index, entity, new FollowComponent { Value = player});
        }
    }

}
