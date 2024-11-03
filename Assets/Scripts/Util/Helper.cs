using Unity.Collections;
using Unity.Entities;

public static class Helper
{
    public static void SetAttribute(this DynamicBuffer<AttributeComponent> buffer, AttributeEnum attributeEnum, float value)
    {
        buffer[(int)attributeEnum] = new AttributeComponent { Value = value };
    }

}
