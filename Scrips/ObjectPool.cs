using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public List<GameObject> pool;

    public GameObject prefab;
    public int poolSize = 5;

    // Start is called before the first frame update
    void Start()
    {
        pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetObjectFromPool()
    {
        foreach (GameObject go in pool)
        {
            if(!go.activeInHierarchy)
            {
                return go;
            }
        }

        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public void ReturnGameObjetToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}
