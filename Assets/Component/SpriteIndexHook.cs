using Unity.Entities;

public struct SpriteIndexHook : IComponentData
{
    public int AtlasValue;
    public int MaterialValue;
    public int BufferValue;
}
