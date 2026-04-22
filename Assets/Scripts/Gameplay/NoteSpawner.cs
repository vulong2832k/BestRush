using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public LevelLoader loader;
    public Transform[] lanes;
    public Transform noteParent;

    public float spawnY;
    public float hitY;
    public float noteSpeed;

    private LevelData _level;
    private int _noteIndex;

    private double _startDspTime;
    private float _travelTime;
    private bool _started;

    private int _activeNotes = 0;
    private bool _isCompleted = false;

    // Score
    private int _hit = 0;
    private int _miss = 0;

    // UI
    public ProgressBarController progressBar;
    [SerializeField] private CompletePanelController completePanel;

    private void Update()
    {
        if (GameManager.Instance.IsPaused) return;
        if (!_started || _level == null) return;

        double songTime = GetSongTime();

        TrySpawnNotes(songTime);

        if (!_isCompleted && _noteIndex >= _level.notes.Count && _activeNotes == 0)
        {
            _isCompleted = true;
            CompleteGame();
        }
    }

    private double GetSongTime()
    {
        return AudioSettings.dspTime - _startDspTime;
    }

    private void TrySpawnNotes(double songTime)
    {
        while (_noteIndex < _level.notes.Count)
        {
            var note = _level.notes[_noteIndex];
            double spawnTime = note.time - _travelTime;

            if (songTime < spawnTime) break;

            Spawn(note);
            _noteIndex++;
        }
    }

    private void Spawn(NoteData data)
    {
        if (data.lane < 0 || data.lane >= lanes.Length) return;

        GameObject note = MultiPool.Instance.Get("note", noteParent);

        Vector3 pos = lanes[data.lane].position;
        pos.y = spawnY;
        note.transform.position = pos;

        note.GetComponent<NotePrefab>().Init(data.lane, data.time, data.duration, hitY, noteSpeed, _startDspTime, this);

        _activeNotes++;
    }

    //GAME START

    public void StartGameFromUI()
    {
        _level = loader.Load();

        _noteIndex = 0;
        _activeNotes = 0;
        _isCompleted = false;

        _hit = 0;
        _miss = 0;

        _travelTime = Mathf.Abs(spawnY - hitY) / noteSpeed;

        GameManager.Instance.StartGame();
        _startDspTime = GameManager.Instance.StartDspTime;

        InputHandler.Instance.startDspTime = _startDspTime;

        progressBar.Init(_level.notes.Count);

        _started = true;

        ComboManager.Instance.ResetAll();
    }

    //NOTE CALLBACK

    public void OnNoteHit()
    {
        _hit++;
        _activeNotes--;

        GameEvents.OnNoteHit?.Invoke();
    }

    public void OnNoteMiss()
    {
        _miss++;
        _activeNotes--;

        GameEvents.OnNoteMiss?.Invoke();
    }

    //COMPLETE

    void CompleteGame()
    {
        float percent = CalculatePercent();
        int miss = _miss;

        completePanel.ShowResult(percent, miss);
    }

    float CalculatePercent()
    {
        int total = _level.notes.Count;
        if (total == 0) return 0;

        return (float)_hit / total;
    }
}