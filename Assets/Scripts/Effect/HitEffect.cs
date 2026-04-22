using UnityEngine;

public class HitEffect : MonoBehaviour, IPoolable
{
    [SerializeField] float lifeTime = 0.6f;

    private string _poolKey;
    private Vector3 _velocity;

    public void Init(string key, Vector3 velocity)
    {
        _poolKey = key;
        _velocity = velocity;
    }

    public void Play(Vector3 pos)
    {
        transform.position = pos;

        CancelInvoke();
        Invoke(nameof(Despawn), lifeTime);
    }

    void Update()
    {
        transform.position += _velocity * Time.deltaTime;
        _velocity *= 0.98f;
    }

    void Despawn()
    {
        MultiPool.Instance.Return(_poolKey, gameObject);
    }

    public void OnSpawn() { }

    public void OnDespawn()
    {
        CancelInvoke();
    }
}