using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public static class FrameAnimationHelper
{
    private static bool isInitialize = false;
    private static Shader animationShader;
    private static EntityArchetype entityArchetype;
    private static EntityManager entityManager;
    private static Dictionary<int, EntityGraphics> dict = new Dictionary<int, EntityGraphics>();
    private static float4 defaultColor = new float4(1, 1, 1, 1);

    public static Entity AddAnimation(Entity entity, Texture atlas, float frameRate, int row = 1, int col = 1)
    {
        var atlasKey = atlas.GetInstanceID();
        if (!dict.TryGetValue(atlasKey, out var entityGraphics))
        {
            entityGraphics = new EntityGraphics(atlasKey, entityManager, animationShader, atlas, GetAtlasUV(row, col));
            dict[atlasKey] = entityGraphics;
        }

        entityManager.AddComponentData(entity, new SpriteIndex { Value = 0 });
        entityManager.AddComponentData(entity, new SpriteColor { Value = defaultColor });
        entityManager.AddComponentData(entity, new SpriteIndexHook { AtlasValue = atlasKey, MaterialValue = entityGraphics.materialId, BufferValue = entityGraphics.GenerateId() });
        entityManager.AddComponentData(entity, new SpriteAnimation { frameRate = frameRate, frameCount = col * row });

        return entity;
    }

    public static Entity CreateEntity(Texture atlas, float frameRate, float scale = 1, int row = 1, int col = 1)
    {
        if (atlas == null)
            return default;

        Initialize();

        var atlasKey = atlas.GetInstanceID();
        if (!dict.TryGetValue(atlasKey, out var entityGraphics))
        {
            entityGraphics = new EntityGraphics(atlasKey, entityManager, animationShader, atlas, GetAtlasUV(row,col));
            dict[atlasKey] = entityGraphics;
        }

        var entity = entityManager.CreateEntity(entityArchetype);
        var localTransform = LocalTransform.FromPositionRotationScale(float3.zero, quaternion.identity, scale);
        entityManager.SetComponentData(entity, localTransform);
        entityManager.SetComponentData(entity, new LocalToWorld { Value = localTransform.ToMatrix() });
        entityManager.SetComponentData(entity, new SpriteIndex { Value = 0 });
        entityManager.SetComponentData(entity, new SpriteColor { Value = defaultColor });
        entityManager.SetComponentData(entity, new SpriteIndexHook { AtlasValue = atlasKey, MaterialValue = entityGraphics.materialId, BufferValue = entityGraphics.GenerateId() });
        entityManager.SetComponentData(entity, new SpriteAnimation { frameRate = frameRate, frameCount = col * row });

        return entity;
    }

    public static void Remove(Entity entity)
    {
        var indexHook = entityManager.GetComponentData<SpriteIndexHook>(entity);
        if (dict.TryGetValue(indexHook.AtlasValue, out var entityGraphics))
        {
            entityGraphics.RemoveBuffer(indexHook.BufferValue);
            entityManager.DestroyEntity(entity);
        }
    }

    public static void Remove(int atlasId, int bufferId)
    {
        if (dict.TryGetValue(atlasId, out var entityGraphics))
        {
            entityGraphics.RemoveBuffer(bufferId);
        }
    }

    private static float4[] GetAtlasUV(int row = 1, int col = 1)
    {
        var spriteCount = row * col;
        float4[] array = new float4[spriteCount];
        float rowStride = 1f / row;
        float colStride = 1f / col;
        for (int _row = 0; _row < row; _row++)
        {
            for (int _col = 0; _col < col; _col++)
            {
                array[_row * col + _col] = new float4(colStride, rowStride, colStride * _col, rowStride * _row);
            }
        }
        return array;
    }

    private static void Initialize()
    {
        if (isInitialize)
            return;

        animationShader = Shader.Find("Instanced/SpriteSheet");
        var world = World.DefaultGameObjectInjectionWorld;
        entityManager = world.EntityManager;
        entityArchetype = entityManager.CreateArchetype(
            typeof(LocalTransform),
            typeof(LocalToWorld),
            typeof(SpriteIndex),
            typeof(SpriteColor),
            typeof(SpriteAnimation),
            typeof(SpriteIndexHook));
        isInitialize = true;
    }
}
