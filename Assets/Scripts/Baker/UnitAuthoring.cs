using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    public int unitId;
    public bool isEnemy;
    public int attack;
    public int attackSpeed;
    public int attackDistance;
    public int hpMax;
    public float moveSpeed;
    public int displayPrefab;
    public Vector3 localPosition;
    public float localScale;
    public Vector3 localEulerAngles;
}

public class UnitBaker : Baker<UnitAuthoring>
{
    public override void Bake(UnitAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new RequestLoadPrefab
        {
            skeletonNode = SkeletonNodeType.RootEntity,
            prefabId = authoring.displayPrefab,
            position = authoring.localPosition,
            scale = authoring.localScale,
            eulerAngles = authoring.localEulerAngles,
        });

        if (authoring.isEnemy)
        {
            AddComponent<FollowComponent>(entity);
        }

        var attributeBuffer = AddBuffer<AttributeComponent>(entity);
        attributeBuffer.Length = (int)AttributeEnum.Max_Count;
        attributeBuffer.SetAttribute(AttributeEnum.Attack, authoring.attack);
        attributeBuffer.SetAttribute(AttributeEnum.AttackSpeed, authoring.attackSpeed);
        attributeBuffer.SetAttribute(AttributeEnum.AttackDistance, authoring.attackDistance);
        attributeBuffer.SetAttribute(AttributeEnum.Hp, authoring.hpMax);
        attributeBuffer.SetAttribute(AttributeEnum.HpMax, authoring.hpMax);
        attributeBuffer.SetAttribute(AttributeEnum.MoveSpeed, authoring.moveSpeed);
    }
}
