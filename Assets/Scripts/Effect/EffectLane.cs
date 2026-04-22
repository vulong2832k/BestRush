using System.Collections.Generic;
using UnityEngine;

public class EffectLane : MonoBehaviour
{
    public List<string> effectKeys;

    public int minSpawn = 2;
    public int maxSpawn = 4;

    public float minForce = 2f;
    public float maxForce = 4f;

    public float spreadAngle = 30f;

    public void Play(Vector3 pos)
    {
        if (effectKeys.Count == 0) return;

        int count = Random.Range(minSpawn, maxSpawn + 1);

        for (int i = 0; i < count; i++)
        {
            string key = effectKeys[Random.Range(0, effectKeys.Count)];

            var obj = MultiPool.Instance.Get(key, transform);
            if (obj == null) continue;

            float angle = Random.Range(-spreadAngle, spreadAngle);
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.down;

            float force = Random.Range(minForce, maxForce);
            Vector3 velocity = dir * force;

            obj.transform.localScale = Vector3.one * Random.Range(0.7f, 1.2f);

            var fx = obj.GetComponent<HitEffect>();
            fx.Init(key, velocity);
            fx.Play(pos);
        }
    }
}