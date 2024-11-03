using Unity.Collections;
using Unity.Entities;

public struct PrefabMapComponent : IComponentData 
{
    public NativeParallelHashMap<int, Entity> dict;
}
