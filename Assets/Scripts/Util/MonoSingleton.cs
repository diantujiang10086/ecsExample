using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static object lockObject = new object();
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    instance = Transform.FindObjectOfType<T>();
                    if (instance == null)
                    {
                        if (instance == null)
                        {
                            var go = new GameObject(typeof(T).Name);
                            GameObject.DontDestroyOnLoad(go);
                            instance = go.AddComponent<T>();
                        }
                    }
                }
                if (instance.transform.parent == null)
                {
                    GameObject.DontDestroyOnLoad(instance.gameObject);
                }
                (instance as MonoSingleton<T>).Initialize();
            }
            return instance;
        }

    }

    protected virtual void Initialize()
    {

    }
}