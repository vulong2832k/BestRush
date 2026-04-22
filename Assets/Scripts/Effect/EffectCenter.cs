using System.Collections.Generic;
using UnityEngine;

public class EffectCenter : MonoBehaviour
{
    public List<string> effectKeys;

    public int minSpawn = 4;
    public int maxSpawn = 7;

    public float minForce = 2f;
    public float maxForce = 5f;

    public void Play()
    {
        if (effectKeys.Count == 0) return;

        Vector3 pos = transform.position;

        int count = Random.Range(minSpawn, maxSpawn + 1);

        for (int i = 0; i < count; i++)
        {
            string key = effectKeys[Random.Range(0, effectKeys.Count)];

            var obj = MultiPool.Instance.Get(key, transform);
            if (obj == null) continue;

            Vector2 dir = Random.insideUnitCircle.normalized;
            float force = Random.Range(minForce, maxForce);

            Vector3 velocity = new Vector3(dir.x, dir.y, 0) * force;

            obj.transform.localScale = Vector3.one * Random.Range(0.7f, 1.3f);
            obj.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

            var fx = obj.GetComponent<HitEffect>();
            fx.Init(key, velocity);
            fx.Play(pos);
        }
    }
}