using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleManager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            
            return instance;
        }
    }

    void Awake()
    {
        if (transform.parent != null) DontDestroyOnLoad(this.transform.root.gameObject);
        else DontDestroyOnLoad(this.gameObject);
    }
}
