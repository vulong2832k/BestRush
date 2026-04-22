using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    public EffectCenter center;
    public EffectLane[] lanes;

    void Awake()
    {
        Instance = this;
    }

    public void PlayCenter()
    {
        center?.Play();
    }

    public void PlayLane(int lane, Vector3 pos)
    {
        if (lane < 0 || lane >= lanes.Length) return;

        lanes[lane]?.Play(pos);
    }
}