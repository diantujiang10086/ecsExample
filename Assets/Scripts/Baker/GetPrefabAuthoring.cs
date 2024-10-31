using Unity.Entities;
using UnityEngine;

public class GetPrefabAuthoring : MonoBehaviour
{
    public GameObject Prefab;
}

public class GetPrefabBaker : Baker<GetPrefabAuthoring>
{
    public override void Bake(GetPrefabAuthoring authoring)
    {
        var entityPrefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic);
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        int.TryParse(authoring.Prefab.name, out var index);
        AddComponent(entity, new EntityPrefabComponent() { Entity = entityPrefab, index = index });
    }
}
