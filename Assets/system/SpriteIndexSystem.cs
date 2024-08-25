using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public partial class SpriteIndexSystem : SystemBase
{
    private EntityQuery query;
    protected override void OnCreate()
    {
        RequireForUpdate<SpriteIndexBuffer>();
        query = GetEntityQuery(typeof(SpriteIndex),typeof(SpriteIndexHook));
    }
    [BurstCompile]
    protected override void OnUpdate()
    {
        var count = query.CalculateEntityCount();
        if (count == 0)
            return;
        var spriteIndices = query.ToComponentDataArray<SpriteIndex>(Allocator.TempJob);
        var materialHooks = query.ToComponentDataArray<SpriteIndexHook>(Allocator.TempJob);
        this.Entities.WithName("SpriteIndexSystem").WithReadOnly(spriteIndices).WithReadOnly(materialHooks)
            .ForEach((ref DynamicBuffer<SpriteIndexBuffer> spriteIndexBuffers, in SpriteIndexHook materialHook) =>
        {
            for (int i = 0; i < spriteIndices.Length; i++)
            {
                if (materialHooks[i].MaterialValue == materialHook.MaterialValue)
                {
                    spriteIndexBuffers[materialHooks[i].BufferValue] = spriteIndices[i].Value;
                }
            }
        })
        .WithDisposeOnCompletion(spriteIndices)
        .WithDisposeOnCompletion(materialHooks)
        .ScheduleParallel();
    }
}
