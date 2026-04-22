using UnityEngine;

public class NotePrefab : MonoBehaviour, IPoolable
{
    private double _startDspTime;
    private float _hitTime;
    private float _duration;

    private float _speed;
    private float _hitY;

    private int _lane;

    [SerializeField] float _despawnOffset = 2f;

    private NoteSpawner _spawner;
    private bool _done = false;

    public float HitY => _hitY;
    public int Lane => _lane;
    public float HitTime => _hitTime;

    public void Init(int lane, float time, float duration, float hitY, float speed, double dspStart, NoteSpawner spawner)
    {
        _lane = lane;
        _hitTime = time;
        _duration = duration;
        _hitY = hitY;
        _speed = speed;
        _startDspTime = dspStart;
        _spawner = spawner;
        _done = false;

        InputHandler.Instance.Register(this);
    }

    void Update()
    {
        if (GameManager.Instance.IsPaused) return;

        double songTime = AudioSettings.dspTime - _startDspTime;

        float timeToHit = _hitTime - (float)songTime;

        float y = _hitY + timeToHit * _speed;

        transform.position = new Vector3(transform.position.x, y, 0);

        if (!_done && y < _hitY - _despawnOffset)
        {
            _done = true;

            InputHandler.Instance.Unregister(this);

            _spawner.OnNoteMiss();

            MultiPool.Instance.Return("note", gameObject);
        }
    }

    public void Hit()
    {
        if (_done) return;

        _done = true;

        InputHandler.Instance.Unregister(this);

        _spawner.OnNoteHit();

        MultiPool.Instance.Return("note", gameObject);
    }

    public void OnSpawn() { }

    public void OnDespawn()
    {
        InputHandler.Instance.Unregister(this);
    }
}