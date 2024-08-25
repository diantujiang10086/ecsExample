using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SpriteFrameAnimation : MonoBehaviour
{
    public Texture atlas;
    public int col;
    public int row;
    public float frameRate;
    public float scale;
    private void Awake()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;
        var entity = FrameAnimationHelper.CreateEntity(atlas, frameRate, scale, row, col);
        var localTransform = entityManager.GetComponentData<LocalTransform>(entity);
        localTransform.Position = transform.position;
        localTransform.Scale = transform.localScale.x;
        localTransform.Rotation = transform.rotation;
        entityManager.SetComponentData<LocalTransform>(entity, localTransform);
    }
}
