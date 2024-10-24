using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class FollowTargetSystem : SystemBase
{
    private partial struct FollowJob : IJobEntity
    {
        public float moveSpeed;
        public float deltaTime;
        public float3 targetPosition;
        void Execute(ref LocalTransform transform,  in EAnimation _)
        {
            float3 direction = math.normalize(targetPosition - transform.Position);
            float3 moveDelta = direction * moveSpeed * deltaTime;

            transform.Position += moveDelta;
        }
    }

    private int index = 0;
    private float moveSpeed = 50;
    private float childMoveSpeed;
    private float3 targetPosition;
    private float3[] positions;
    private float stopThresholdSqr = 0.01f;
    private Transform cube;
    protected override void OnCreate()
    {
        cube =  GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        positions = new float3[4];
        positions[0] = GameObject.Find("0").transform.position;
        positions[1] = GameObject.Find("1").transform.position;
        positions[2] = GameObject.Find("2").transform.position;
        positions[3] = GameObject.Find("3").transform.position;
        targetPosition = positions[index++];
    }

    protected override void OnStartRunning()
    {
        moveSpeed = Spawn.Instance.moveSpeed;
        childMoveSpeed = Spawn.Instance.childMoveSpeed;
    }

    protected override void OnUpdate()
    {
        float3 nextPosition = positions[index];
        targetPosition = Vector3.MoveTowards(targetPosition, nextPosition, moveSpeed * SystemAPI.Time.DeltaTime);

        float sqrDistance = math.lengthsq(nextPosition - targetPosition);

        if (sqrDistance < stopThresholdSqr)
        {
            index = ++index % 4;
        }
        cube.position = targetPosition;
        new FollowJob 
        { 
            targetPosition = targetPosition,
            moveSpeed = childMoveSpeed,
            deltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }

}