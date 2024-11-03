using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct RequestLoadPrefabSystem : ISystem
{
    private EntityQuery entityQuery;
    void OnCreate(ref SystemState state) 
    {
        state.RequireForUpdate<RequestLoadPrefab>();
        entityQuery = state.GetEntityQuery(typeof(RequestLoadPrefab));
    }

    void OnUpdate(ref SystemState state) 
    {
        var components = entityQuery.ToComponentDataArray<RequestLoadPrefab>(Allocator.TempJob);
        var entities = entityQuery.ToEntityArray(Allocator.TempJob);
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        var job = new ExecuteJob
        {
            entities = entities,
            components = components,
            prefabDict = SystemAPI.GetSingleton<PrefabMapComponent>().dict,
            ecb = ecb,
            skeletonNodeLookup = SystemAPI.GetBufferLookup<SkeletonNode>()
        };
        var jobHandle = job.Schedule(components.Length, 128);
        jobHandle.Complete();

        components.Dispose();
        entities.Dispose();
    }

    [BurstCompile]
    struct ExecuteJob : IJobParallelFor
    {
        [ReadOnly] public NativeParallelHashMap<int, Entity> prefabDict;
        [ReadOnly] public NativeArray<RequestLoadPrefab> components;
        [ReadOnly] public NativeArray<Entity> entities;
        [ReadOnly] public BufferLookup<SkeletonNode> skeletonNodeLookup;
        public EntityCommandBuffer.ParallelWriter ecb;

        [BurstCompile]
        public void Execute(int index)
        {
            var entity = entities[index];
            var component = components[index];

            if (prefabDict.TryGetValue(component.prefabId, out var prefabEntity))
            {
                Entity parentEntity = default;
                if(component.skeletonNode == SkeletonNodeType.RootEntity)
                {
                    parentEntity = entity;
                }
                else if (skeletonNodeLookup.TryGetBuffer(entity, out var buffer))
                {
                    if(buffer.Length > (int)component.skeletonNode)
                    {
                        parentEntity = buffer[(int)component.skeletonNode].Value;
                    }
                    else
                    {
                        Log.Warning($"node not found:{component.skeletonNode}");
                    }
                }
                else
                {
                    Log.Warning($"Do not create an object if the parent node is empty:{component.prefabId}");
                }

                if (parentEntity != Entity.Null)
                {
                    var displayEntity = ecb.Instantiate(index, prefabEntity);
                    ecb.AddComponent(index, displayEntity, new Parent { Value = parentEntity });
                    quaternion rotate = quaternion.Euler(component.eulerAngles);
                    var localTransform = LocalTransform.FromPositionRotationScale(component.position, rotate, component.scale);
                    ecb.SetComponent(index, displayEntity, localTransform);
                }
            }
            else
            {
                Log.Warning($"Failed to find the prefab:{component.prefabId}");
            }
            ecb.RemoveComponent<RequestLoadPrefab>(index, entity);
        }
    }
}
