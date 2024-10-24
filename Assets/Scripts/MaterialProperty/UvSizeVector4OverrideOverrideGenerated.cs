using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_UvSize")]
    struct UvSizeVector4Override : IComponentData
    {
        public float4 Value;
    }
}
