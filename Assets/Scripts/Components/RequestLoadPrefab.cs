using Unity.Entities;
using Unity.Mathematics;

public struct RequestLoadPrefab : IComponentData
{
    public SkeletonNodeType skeletonNode;
    public int prefabId;
    public float3 position;
    public float scale;
    public float3 eulerAngles;
}
