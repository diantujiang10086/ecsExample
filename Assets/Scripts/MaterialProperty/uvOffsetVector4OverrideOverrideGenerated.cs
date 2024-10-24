using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_uvOffset")]
    struct uvOffsetVector4Override : IComponentData
    {
        public float4 Value;
    }
}
