using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;

public partial class SpawnSystem : SystemBase
{

    EntityQuery entityQuery;
    protected override void OnCreate()
    {
        entityQuery = GetEntityQuery(typeof(EAnimation));
    }
    protected override void OnUpdate()
    {
        var count = entityQuery.CalculateEntityCount();
        if (count == 0)
            return;
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(EntityManager.WorldUnmanaged);
        var arrays = entityQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
        var entity = arrays[0];

        var filterSettings = RenderFilterSettings.Default;
        filterSettings.ShadowCastingMode = ShadowCastingMode.Off;
        filterSettings.ReceiveShadows = false;

        var defaultSetting = RenderFilterSettings.Default;
        defaultSetting.ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        defaultSetting.ReceiveShadows = false;
        defaultSetting.StaticShadowCaster = false;
        EntityManager.SetSharedComponent(entity, defaultSetting);

        int length = Spawn.Instance.count;
        var job = new SpawnJob
        {
            offset = Spawn.Instance.offset,
            count = Spawn.Instance.colCount,
            ecb = ecb.AsParallelWriter(),
            archetype = entity,
            random = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue))
        };
        job.Schedule(length, 64).Complete();

        this.Enabled = false;
    }

    public struct SpawnJob : IJobParallelFor
    {
        [ReadOnly] public float3 offset;
        [ReadOnly] public int count;
        public Entity archetype;
        public EntityCommandBuffer.ParallelWriter ecb;
        public Random random;
        public void Execute(int index)
        {
            var entity = ecb.Instantiate(index, archetype);
            ecb.SetComponent(index, entity, LocalTransform.FromPosition(offset + new Unity.Mathematics.float3(index % count, index / count, 0)));
            ecb.SetComponent(index, entity, new EAnimation { sample = random.NextFloat(5, 60) });
        }
    }
}
