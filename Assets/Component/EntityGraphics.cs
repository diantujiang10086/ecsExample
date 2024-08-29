using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EntityGraphics : IComponentData
{
    private const float clearTime = 1;
    public int materialId;
    public int atlasId;
    public Entity bufferEntity;
    public int instanceCount;
    private ComputeBuffer uvBuffer;
    private ComputeBuffer indexBuffer;
    private ComputeBuffer matrixBuffer;
    private ComputeBuffer colorsBuffer;
    private ComputeBuffer argsBuffer;
    private Material material;
    private uint[] args;
    private EntityManager entityManager;
    private DynamicGenerateIDList dynamicGenerateIDList;
    private float clearTimer = clearTime;
    public EntityGraphics()
    {

    }
    public EntityGraphics(int atlasKey, EntityManager entityManager, Shader shader, Texture atlas, float4[] atlasUv)
    {
        atlasId = atlasKey;
        bufferEntity = entityManager.CreateEntity(typeof(SpriteIndexBuffer), typeof(SpriteMatrixBuffer), typeof(SpriteColorBuffer), typeof(SpriteIndexHook));
        dynamicGenerateIDList = new DynamicGenerateIDList();
        this.entityManager = entityManager;
        material = new Material(shader);
        material.mainTexture = atlas;

        materialId = bufferEntity.Index;
        args = new uint[5] { 6, 0, 0, 0, 0 };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

#if UNITY_EDITOR
        entityManager.SetName(bufferEntity, $"[EntityGraphics]{bufferEntity.Index}");
#endif
        entityManager.SetComponentData(bufferEntity, new SpriteIndexHook { MaterialValue = bufferEntity.Index });
        entityManager.AddComponentObject(bufferEntity, this);

        uvBuffer = new ComputeBuffer(atlasUv.Length, 16);
        uvBuffer.SetData(atlasUv);
        material.SetBuffer("uvBuffer", uvBuffer);
        ComputeBufferInitialize();
    }

    public int GenerateId()
    {
        var bufferId = dynamicGenerateIDList.GenerateID();
        var indexBuffer = entityManager.GetBuffer<SpriteIndexBuffer>(bufferEntity);
        var matrixBuffer = entityManager.GetBuffer<SpriteMatrixBuffer>(bufferEntity);
        var colorBuffer = entityManager.GetBuffer<SpriteColorBuffer>(bufferEntity);

        if (indexBuffer.Length <= bufferId)
        {
            indexBuffer.Add(new SpriteIndexBuffer { });
            matrixBuffer.Add(new SpriteMatrixBuffer { });
            colorBuffer.Add(new SpriteColorBuffer { });
        }

        instanceCount++;
        return bufferId;
    }

    public void RemoveBuffer(int bufferId)
    {
        var colorBuffer = entityManager.GetBuffer<SpriteColorBuffer>(bufferEntity);
        if (bufferId >= colorBuffer.Length)
            return;
        dynamicGenerateIDList.Recycle(bufferId);
        colorBuffer[bufferId] = default(float4);
        instanceCount--;
    }

    public void UpdateBuffer(EntityManager entityManager)
    {
        var spriteIndexBuffer = entityManager.GetBuffer<SpriteIndexBuffer>(bufferEntity);
        var instanceCount = spriteIndexBuffer.Length;
        if (spriteIndexBuffer.Length == 0)
            return;

        indexBuffer.Release();
        matrixBuffer.Release();
        colorsBuffer.Release();

        indexBuffer = new ComputeBuffer(instanceCount, sizeof(int));
        indexBuffer.SetData(spriteIndexBuffer.Reinterpret<int>().AsNativeArray());

        matrixBuffer = new ComputeBuffer(instanceCount, 16);
        matrixBuffer.SetData(entityManager.GetBuffer<SpriteMatrixBuffer>(bufferEntity).Reinterpret<float4>().AsNativeArray());

        colorsBuffer = new ComputeBuffer(instanceCount, 16);
        colorsBuffer.SetData(entityManager.GetBuffer<SpriteColorBuffer>(bufferEntity).Reinterpret<float4>().AsNativeArray());

        args[1] = (uint)instanceCount;
        argsBuffer.SetData(args);

        material.SetBuffer("indexBuffer", indexBuffer);
        material.SetBuffer("matrixBuffer", matrixBuffer);
        material.SetBuffer("colorsBuffer", colorsBuffer);
    }

    public void Draw(Mesh mesh, Bounds bounds)
    {
        if (instanceCount == 0)
        {
            clearTimer -= Time.deltaTime;
            if (clearTimer <= 0)
            {
                ComputeBufferInitialize();
            }
            return;
        }

        Graphics.DrawMeshInstancedIndirect(
            mesh,
            0,
            material,
            bounds,
            argsBuffer,
            receiveShadows: false,
            castShadows: UnityEngine.Rendering.ShadowCastingMode.Off,
            lightProbeUsage: UnityEngine.Rendering.LightProbeUsage.Off);

        clearTimer = clearTime;
    }


    public void Release()
    {
        uvBuffer.Release();
        indexBuffer.Release();
        matrixBuffer.Release();
        colorsBuffer.Release();
        argsBuffer.Release();
    }

    private void ComputeBufferInitialize()
    {
        indexBuffer = new ComputeBuffer(1, sizeof(int));
        matrixBuffer = new ComputeBuffer(1, 16);
        colorsBuffer = new ComputeBuffer(1, 16);
    }
}
