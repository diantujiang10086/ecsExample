using UnityEngine;

public class Spawn : MonoBehaviour
{
    public float moveSpeed;
    public float childMoveSpeed;
    public Vector3 offset;
    public int count;
    public int colCount;

    public static Spawn Instance;
    private void Awake()
    {
        Instance = this;
    }
}
