using UnityEngine;
using TMPro;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text totalNoteHitText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text comboNumberText;

    private int _totalHit = 0;
    private int _combo = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        GameEvents.OnNoteHit += OnHit;
        GameEvents.OnNoteMiss += OnMiss;
    }

    private void OnDisable()
    {
        GameEvents.OnNoteHit -= OnHit;
        GameEvents.OnNoteMiss -= OnMiss;
    }
    public void OnHit()
    {
        _totalHit++;
        _combo++;

        UpdateUI();
    }

    public void OnMiss()
    {
        _combo = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        totalNoteHitText.text = $"{_totalHit}";

        comboNumberText.text = _combo.ToString();

        string text = GetComboText(_combo);
        comboText.text = text;

        comboText.color = GetComboColor(_combo);
    }

    string GetComboText(int combo)
    {
        if (combo >= 200) return "Legendary";
        if (combo >= 150) return "Perfect";
        if (combo >= 100) return "Good";
        if (combo >= 50) return "Great";

        return "";
    }
    Color GetComboColor(int combo)
    {
        if (combo >= 200) return new Color(1f, 0.4f, 0f);
        if (combo >= 150) return new Color(1f, 0.6f, 0f); 
        if (combo >= 100) return new Color(0.5f, 1f, 0.5f);
        if (combo >= 50) return Color.yellow;

        return Color.white;
    }
    public void ResetAll()
    {
        _totalHit = 0;
        _combo = 0;
        UpdateUI();
    }
}