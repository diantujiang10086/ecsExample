using Unity.Collections;
using Unity.Entities;

public static class Helper
{
    public static void SetAttribute(this DynamicBuffer<AttributeComponent> buffer, AttributeEnum attributeEnum, float value)
    {
        buffer[(int)attributeEnum] = new AttributeComponent { Value = value };
    }

    public static Entity GetPlayer(EntityQuery entityQuery)
    {
        var entities = entityQuery.ToEntityArray(Allocator.TempJob);
        if(entities.Length > 0)
        {
            return entities[0];
        }
        return default;
    }
}
