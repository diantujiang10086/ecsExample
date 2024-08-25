using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class Example : MonoBehaviour
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
            localTransform.Position = new Unity.Mathematics.float3(Random.Range(-100,100), Random.Range(-100, 100), Random.Range(-100, 100));
            entityManager.SetComponentData<LocalTransform>(entity, localTransform);
        }
    }
}
