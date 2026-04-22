using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;

    private List<NotePrefab> _activeNotes = new List<NotePrefab>();

    public double startDspTime;

    public float perfectWindow = 0.08f;
    public float goodWindow = 0.15f;

    private void Awake()
    {
        Instance = this;
    }

    public void Register(NotePrefab note)
    {
        _activeNotes.Add(note);
    }

    public void Unregister(NotePrefab note)
    {
        _activeNotes.Remove(note);
    }

    private void Update()
    {
        HandleTouch();
        HandleKeyboard();
    }

    void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.A)) TryHit(0);
        if (Input.GetKeyDown(KeyCode.S)) TryHit(1);
        if (Input.GetKeyDown(KeyCode.D)) TryHit(2);
        if (Input.GetKeyDown(KeyCode.F)) TryHit(3);
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);

        if (t.phase != TouchPhase.Began) return;

        int lane = GetLaneFromTouch(t.position);
        TryHit(lane);
    }

    private int GetLaneFromTouch(Vector2 pos)
    {
        float laneWidth = Screen.width / 4f;
        return Mathf.Clamp((int)(pos.x / laneWidth), 0, 3);
    }

    private void TryHit(int lane)
    {
        double songTime = AudioSettings.dspTime - startDspTime;

        NotePrefab target = GetClosestNote(lane);
        if (target == null) return;

        float diff = Mathf.Abs((float)songTime - target.HitTime);

        if (diff > goodWindow)
        {
            return;
        }

        Vector3 lanePos = new Vector3(target.transform.position.x, target.HitY, 0);

        target.Hit();

        if (diff <= perfectWindow)
        {
            EffectManager.Instance?.PlayCenter();
            EffectManager.Instance?.PlayLane(lane, lanePos);
        }
        else
        {
            EffectManager.Instance?.PlayCenter();
        }
    }

    private NotePrefab GetClosestNote(int lane)
    {
        NotePrefab best = null;
        float minDiff = float.MaxValue;

        float currentTime = (float)(AudioSettings.dspTime - startDspTime);

        foreach (var note in _activeNotes)
        {
            if (note.Lane != lane) continue;

            float diff = note.HitTime - currentTime;

            if (diff < -0.05f) continue;

            if (diff < -goodWindow) continue;

            float absDiff = Mathf.Abs(diff);

            if (absDiff < minDiff)
            {
                minDiff = absDiff;
                best = note;
            }
        }

        return best;
    }
}