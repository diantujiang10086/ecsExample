using Unity.Entities;
using UnityEngine;

public class MapAuthoring : MonoBehaviour
{
    public int player;
    public Vector3 playerPosition;
    public int spawnEnemy;
    public float spawnInterval;
    public int spawnCount;
    public float range;

}

public class MapBaker : Baker<MapAuthoring>
{
    public override void Bake(MapAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new MapComponent 
        {
            entity = entity,
            playerId = authoring.player,
            playerPosition = authoring.playerPosition,
            spawnInterval = authoring.spawnInterval,
            spawnCount = authoring.spawnCount,
            spawnEnemy = authoring.spawnEnemy,
            range = authoring.range
        });
        AddComponent(entity, new TimeComponent { Value = authoring.spawnInterval });
    }
}