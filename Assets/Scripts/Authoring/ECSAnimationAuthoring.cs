using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class ECSAnimationAuthoring : MonoBehaviour
{
    public int row;
    public int col;
    public int sample;
}

public struct Atlas : IComponentData
{
    public int row;
    public int col;
}
public struct EAnimation : IComponentData
{
    public int index;
    public float sample;
    public float time;
}
public class ECSAnimationBaker : Baker<ECSAnimationAuthoring>
{
    public override void Bake(ECSAnimationAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Atlas { row = authoring.row, col = authoring.col });
        AddComponent(entity, new EAnimation { index = 0, sample = authoring.sample });
        AddComponent<uvOffsetVector4Override>(entity);
        AddComponent(entity, new UvSizeVector4Override { Value = new float4(0.3333f, 0.25f, 0, 0) });
    }
}