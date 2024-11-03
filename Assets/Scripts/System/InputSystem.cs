using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct InputSystem : ISystem
{
    private EntityQuery entityQuery;
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
        entityQuery = state.GetEntityQuery(typeof(PlayerComponent));
    }

    void OnUpdate(ref SystemState state)
    {
        var Horizontal = Input.GetAxisRaw("Horizontal");
        var Vertical = Input.GetAxisRaw("Vertical");

        var player = Helper.GetPlayer(entityQuery);
        if (player == Entity.Null)
            return;
        var attribute = state.EntityManager.GetBuffer<AttributeComponent>(player);
        var transform = state.EntityManager.GetComponentData<LocalTransform>(player);
        var offset = new Unity.Mathematics.float3(Horizontal, 0, Vertical) * (attribute[(int)AttributeEnum.MoveSpeed].Value * SystemAPI.Time.DeltaTime);
        transform.Position += offset;
        state.EntityManager.SetComponentData(player, transform);
    }
}