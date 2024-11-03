using Unity.Entities;
using UnityEngine;

public class SkeletonNodeAuthoring : MonoBehaviour
{
    public Transform[] nodes;
}

public class SkeletonNodeBaker : Baker<SkeletonNodeAuthoring>
{
    public override void Bake(SkeletonNodeAuthoring authoring)
    {
        var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        if (authoring.nodes != null)
        {
            var buffer = AddBuffer<SkeletonNode>(entity);
            buffer.Length = authoring.nodes.Length;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = new SkeletonNode { Value = GetEntity(authoring.nodes[i], TransformUsageFlags.Dynamic) };
            }
        }
    }
}

