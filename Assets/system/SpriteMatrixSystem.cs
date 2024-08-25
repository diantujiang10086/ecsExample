using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
[BurstCompile]
public partial class SpriteMatrixSystem : SystemBase
{
    private EntityQuery query;
    protected override void OnCreate()
    {
        RequireForUpdate<SpriteMatrixBuffer>();
        query = GetEntityQuery(typeof(LocalTransform), typeof(SpriteIndexHook));
    }
    [BurstCompile]
    protected override void OnUpdate()
    {
        var count = query.CalculateEntityCount();
        if (count == 0)
            return;
        var localTransform = query.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var materialHooks = query.ToComponentDataArray<SpriteIndexHook>(Allocator.TempJob);
        this.Entities.WithName("MatrixSystem").WithReadOnly(localTransform).WithReadOnly(materialHooks)
            .ForEach((ref DynamicBuffer<SpriteMatrixBuffer> matrixBuffer, in SpriteIndexHook materialHook) =>
            {
                for (int i = 0; i < localTransform.Length; i++)
                {
                    if (materialHooks[i].MaterialValue == materialHook.MaterialValue)
                    {
                        var transform = localTransform[i];
                        var position = transform.Position;

                        matrixBuffer[materialHooks[i].BufferValue] =  new float4(
                            position.x, 
                            position.y, 
                            math.degrees(math.Euler(transform.Rotation).z), 
                            transform.Scale);
                    }
                }
            })
        .WithDisposeOnCompletion(localTransform)
        .WithDisposeOnCompletion(materialHooks)
        .ScheduleParallel();

    }
}
