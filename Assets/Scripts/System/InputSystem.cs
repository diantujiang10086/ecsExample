using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct InputSystem : ISystem
{
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponent>();
    }

    void OnUpdate(ref SystemState state)
    {
        var Horizontal = Input.GetAxisRaw("Horizontal");
        var Vertical = Input.GetAxisRaw("Vertical");

        if(!SystemAPI.TryGetSingleton<PlayerComponent>(out var playerComponent))
            return;


        var player = playerComponent.Value;
        
        var attribute = state.EntityManager.GetBuffer<AttributeComponent>(player);
        var transform = state.EntityManager.GetComponentData<LocalTransform>(player);
        var offset = new Unity.Mathematics.float3(Horizontal, 0, Vertical) * (attribute[(int)AttributeEnum.MoveSpeed].Value * SystemAPI.Time.DeltaTime);
        transform.Position += offset;
        state.EntityManager.SetComponentData(player, transform);
    }
}