using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ECSExample : MonoBehaviour
{
    public Texture atlas;
    public int col;
    public int row;
    public float frameRate;
    public float scale;

    public int count;

    private void Awake()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;

        for (int i = 0; i < count; i++)
        {
            var entity = FrameAnimationHelper.CreateEntity(atlas, Random.Range(5, 30), scale, row, col);
            var localTransform = entityManager.GetComponentData<LocalTransform>(entity);
            localTransform.Position = new Unity.Mathematics.float3(i % 100, i / 100, 0);
            entityManager.SetComponentData<LocalTransform>(entity, localTransform);
        }
    }
}
