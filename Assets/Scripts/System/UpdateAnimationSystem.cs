using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class UpdateAnimationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        this.Entities.ForEach((ref EAnimation animtion, ref uvOffsetVector4Override uvOffsetVector4Override, in UvSizeVector4Override size, in Atlas atlas) => 
        {
            animtion.time += SystemAPI.Time.DeltaTime;
            animtion.index = (int)(animtion.sample * animtion.time) % (atlas.col * atlas.row);

            uvOffsetVector4Override.Value = new Unity.Mathematics.float4((animtion.index % atlas.col) * size.Value.x, (animtion.index / atlas.col) * size.Value.y, 0,0);


        }).ScheduleParallel();
    }
}
