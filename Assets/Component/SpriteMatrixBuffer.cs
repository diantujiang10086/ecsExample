using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(16)]
public struct SpriteMatrixBuffer : IBufferElementData
{
    public static implicit operator float4(SpriteMatrixBuffer e) { return e.matrix; }
    public static implicit operator SpriteMatrixBuffer(float4 e) { return new SpriteMatrixBuffer { matrix = e }; }
    public float4 matrix;
}
