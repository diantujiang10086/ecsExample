using Unity.Entities;
using Unity.Mathematics;

public struct MapComponent : IComponentData
{
    public Entity entity;
    public int playerId;
    public float3 playerPosition;
    public float spawnInterval;
    public int spawnCount;
    public int spawnEnemy;
    public float range;
}
