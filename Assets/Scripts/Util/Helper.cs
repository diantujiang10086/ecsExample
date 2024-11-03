using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public static class Helper
{
    public static void SetAttribute(this DynamicBuffer<AttributeComponent> buffer, AttributeEnum attributeEnum, float value)
    {
        buffer[(int)attributeEnum] = new AttributeComponent { Value = value };
    }

    public static bool ArePointsClose(float3 p1, float3 p2, float threshold = 0.01f)
    {
        float distanceSquared = math.distancesq(p1, p2);
        return distanceSquared <= threshold * threshold;
    }
}
