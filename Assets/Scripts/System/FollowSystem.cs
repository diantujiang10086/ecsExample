﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct FollowSystem : ISystem
{
    private EntityQuery entityQuery;
    void OnCreate(ref SystemState state) 
    {
        state.RequireForUpdate<FollowComponent>();
        entityQuery = state.GetEntityQuery(typeof(FollowComponent));
    }
    [BurstCompile]
    void OnUpdate(ref SystemState state) 
    {
        var entities = entityQuery.ToEntityArray(Allocator.TempJob);
        var job = new FollowJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
            followComponentLookup = SystemAPI.GetComponentLookup<FollowComponent>(),
            attributeComponentLookup = SystemAPI.GetBufferLookup<AttributeComponent>(),
            entities = entities,
        };

        var jobHandle = job.Schedule(entities.Length, 128);
        jobHandle.Complete();

        entities.Dispose();
    }

    [BurstCompile]
    struct FollowJob : IJobParallelFor
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public ComponentLookup<LocalTransform> localTransformLookup;
        [ReadOnly] public ComponentLookup<FollowComponent> followComponentLookup;
        [ReadOnly] public BufferLookup<AttributeComponent> attributeComponentLookup;
        [ReadOnly] public NativeArray<Entity> entities;
        public EntityCommandBuffer.ParallelWriter ecb;
        [BurstCompile]
        public void Execute(int index)
        {
            var entity = entities[index];
            localTransformLookup.TryGetComponent(entity, out var enemyTransform);
            followComponentLookup.TryGetComponent(entity, out var followComponent);

            localTransformLookup.TryGetComponent(followComponent.Value, out var targetTransform);

            if(Helper.ArePointsClose(enemyTransform.Position, targetTransform.Position))
            {
                return;
            }

            attributeComponentLookup.TryGetBuffer(entity, out var attributeComponent);

            enemyTransform.Position = math.lerp(enemyTransform.Position, targetTransform.Position, attributeComponent[(int)AttributeEnum.MoveSpeed].Value * deltaTime);

            ecb.SetComponent(index, entity, enemyTransform);
        }
    }
}
