using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    public GameObject prefab;
    public int count;

    private void Awake()
    {
        for (int i = 0; i < count; i++)
        {
            var go = GameObject.Instantiate(prefab);
            go.transform.position = new Vector3((i%100)*2,(i/100)*3);
        }
    }
}
