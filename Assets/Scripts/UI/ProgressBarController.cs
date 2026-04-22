using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    public Image fillImage;

    private int _totalNotes;
    private int _hitCount;

    public void Init(int totalNotes)
    {
        this._totalNotes = totalNotes;
        this._hitCount = 0;

        UpdateBar();
    }

    private void OnEnable()
    {
        GameEvents.OnNoteHit += OnNoteHit;
    }

    private void OnDisable()
    {
        GameEvents.OnNoteHit -= OnNoteHit;
    }

    private void OnNoteHit()
    {
        _hitCount++;
        UpdateBar();
    }

    private void UpdateBar()
    {
        Debug.Log($"Hit: {_hitCount} / Total: {_totalNotes}");

        float percent = (float)_hitCount / _totalNotes;
        fillImage.fillAmount = percent;
    }
}