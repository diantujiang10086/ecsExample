using Unity.Entities;

public struct SpriteAnimation : IComponentData
{
    public float elapsedFrames;
    public float frameRate;
    public int frameCount;
}
