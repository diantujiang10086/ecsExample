using Unity.Collections;
using Unity.Entities;

public static class GlobalData
{
    public static NativeHashMap<int, Entity> prefabDict;

    public static void Initialize()
    {
        prefabDict = new NativeHashMap<int, Entity>(1, Allocator.Persistent);
    }

    public static void UpdatePrefab(NativeArray<EntityPrefabComponent> components)
    {
        prefabDict.Clear();
        foreach (var component in components)
        {
            prefabDict[component.index] = component.Entity;
        }
    }

    public static void Clear()
    {
        prefabDict.Dispose();
    }
}