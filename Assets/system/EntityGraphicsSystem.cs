using Unity.Entities;
using UnityEngine;

public partial class EntityGraphicsSystem : SystemBase
{
    private Bounds bounds;
    private Mesh mesh;
    protected override void OnCreate()
    {
        bounds.center = Vector3.zero;
        bounds.min = new Vector3(-1000, -1000, -1000);
        bounds.max = new Vector3(1000, 1000, 1000);
        mesh = MeshExtension.Quad();
        RequireForUpdate<EntityGraphics>();
    }

    protected override void OnDestroy()
    {
        foreach (var (graphics, _) in SystemAPI.Query<EntityGraphics>().WithEntityAccess())
        {
            graphics.Release();
        }
    }

    protected override void OnUpdate()
    {
        foreach (var (graphics,_) in SystemAPI.Query<EntityGraphics>().WithEntityAccess())
        {
            graphics.UpdateBuffer(EntityManager);
            graphics.Draw(mesh, bounds);
        }
    }
}
