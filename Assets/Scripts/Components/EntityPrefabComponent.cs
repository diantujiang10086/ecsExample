using Unity.Entities;

public struct EntityPrefabComponent : IComponentData
{
    public int index;
    public Entity Entity;
}
