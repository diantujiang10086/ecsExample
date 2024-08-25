using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial class SpriteAnimationSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        Entities.ForEach((ref SpriteAnimation animation, ref SpriteIndex spriteIndex) => 
        {
            if (animation.frameCount <= 0 || animation.frameRate <= 0)
                return;

            animation.elapsedFrames += SystemAPI.Time.DeltaTime;
            int currentFrame = (int)((animation.elapsedFrames * animation.frameRate) % animation.frameCount);
            spriteIndex.Value = currentFrame;
        }).ScheduleParallel();
    }
}
