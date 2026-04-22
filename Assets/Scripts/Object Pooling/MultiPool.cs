using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}

public class MultiPool : MonoBehaviour
{
    public static MultiPool Instance;

    [System.Serializable]
    public class Pool
    {
        public string key;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> _poolDict;
    private Dictionary<string, GameObject> _prefabDict;

    void Awake()
    {
        Instance = this;

        _poolDict = new Dictionary<string, Queue<GameObject>>();
        _prefabDict = new Dictionary<string, GameObject>();

        foreach (var pool in pools)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            _poolDict.Add(pool.key, queue);
            _prefabDict.Add(pool.key, pool.prefab);
        }
    }

    public GameObject Get(string key, Transform parent)
    {
        if (!_poolDict.ContainsKey(key))
        {
            Debug.LogError("Thiếu key trong pool: " + key);
            return null;
        }

        GameObject obj = _poolDict[key].Count > 0 ? _poolDict[key].Dequeue() : Instantiate(_prefabDict[key], parent);

        obj.SetActive(true);
        obj.transform.SetParent(parent);

        var poolable = obj.GetComponent<IPoolable>();
        poolable?.OnSpawn();

        return obj;
    }

    public void Return(string key, GameObject obj)
    {
        if (!_poolDict.ContainsKey(key))
        {
            Debug.LogError("Pool not found: " + key);
            return;
        }

        var poolable = obj.GetComponent<IPoolable>();
        poolable?.OnDespawn();

        obj.SetActive(false);
        obj.transform.SetParent(transform);

        _poolDict[key].Enqueue(obj);
    }
}