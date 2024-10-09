using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbleZonePool : MonoBehaviour
{
    public static AbleZonePool Instance;

    [SerializeField] GameObject prefab;
    [SerializeField] Queue<GameObject> pool;
    [SerializeField] int size = 10;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        pool = new Queue<GameObject>(size);
        for (int i = 0; i < size; i++) 
        {
            GameObject instance = Instantiate(prefab);
            instance.SetActive(false);
            instance.transform.SetParent(transform);
            pool.Enqueue(instance);
        }
    }

    public GameObject GetPool(Vector3 pos)
    {
        if (pool.Count > 0)
        {
            GameObject instance = pool.Dequeue();
            instance.transform.position = pos;
            instance.SetActive(true);
            return instance;
        }
        else
        {
            GameObject instance = Instantiate(prefab);
            instance.transform.SetParent(transform);
            instance.transform.position= pos;
            return instance;
        }
    }
    public void ReturnPool(GameObject instance)
    {
        instance.SetActive(false);
        pool.Enqueue(instance);
    }
}
