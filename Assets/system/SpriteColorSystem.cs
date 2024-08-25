using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
[BurstCompile]
public partial class SpriteColorSystem : SystemBase
{
    private EntityQuery query;
    protected override void OnCreate()
    {
        RequireForUpdate<SpriteColor>();
        query = GetEntityQuery(typeof(SpriteColor), typeof(SpriteIndexHook));
    }
    [BurstCompile]
    protected override void OnUpdate()
    {
        var count = query.CalculateEntityCount();
        if (count == 0)
            return;
        var spriteColor = query.ToComponentDataArray<SpriteColor>(Allocator.TempJob);
        var materialHooks = query.ToComponentDataArray<SpriteIndexHook>(Allocator.TempJob);
        this.Entities.WithName("MatrixSystem").WithReadOnly(spriteColor).WithReadOnly(materialHooks)
            .ForEach((ref DynamicBuffer<SpriteColorBuffer> matrixBuffer, in SpriteIndexHook materialHook) =>
            {
                for (int i = 0; i < spriteColor.Length; i++)
                {
                    if (materialHooks[i].MaterialValue == materialHook.MaterialValue)
                    {
                        matrixBuffer[materialHooks[i].BufferValue] = spriteColor[i].Value;
                    }
                }
            })
        .WithDisposeOnCompletion(spriteColor)
        .WithDisposeOnCompletion(materialHooks)
        .ScheduleParallel();

    }
}
