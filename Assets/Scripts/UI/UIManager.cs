using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    [SerializeField] private CanvasGroup _pausePanel;
    [SerializeField] private CanvasGroup _settingPanel;
    [SerializeField] private CanvasGroup _selectMusicPanel;

    [Header("Audio")]
    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;

    private bool _isMusicOn = true;
    private bool _isSfxOn = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        HidePanel(_pausePanel);
        HidePanel(_settingPanel);
        ShowPanel(_selectMusicPanel);

        ApplyMusicVolume();
        ApplySFXVolume();
    }

    // ================= UI CORE =================

    void ShowPanel(CanvasGroup panel)
    {
        panel.alpha = 0;
        panel.DOFade(1, 0.3f).SetUpdate(true);

        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    void HidePanel(CanvasGroup panel)
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }

    void FadeOut(CanvasGroup panel)
    {
        panel.DOFade(0, 0.2f).SetUpdate(true)
        .OnComplete(() =>
        {
            panel.interactable = false;
            panel.blocksRaycasts = false;
        });
    }

    // ================= PAUSE =================

    public void OpenPause()
    {
        GameManager.Instance.PauseGame();
        ShowPanel(_pausePanel);
    }

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
        FadeOut(_pausePanel);
    }

    // ================= SETTING =================

    public void OpenSetting()
    {
        FadeOut(_pausePanel);
        ShowPanel(_settingPanel);
    }

    public void BackToPause()
    {
        FadeOut(_settingPanel);
        ShowPanel(_pausePanel);
    }

    // ================= SELECT MUSIC =================

    public void OpenSelectMusic()
    {
        ShowPanel(_selectMusicPanel);
    }

    public void CloseSelectMusic()
    {
        FadeOut(_selectMusicPanel);
    }

    // ================= AUDIO =================

    public void ToggleMusic()
    {
        _isMusicOn = !_isMusicOn;
        ApplyMusicVolume();
    }

    public void ToggleSFX()
    {
        _isSfxOn = !_isSfxOn;
        ApplySFXVolume();
    }

    public void SetMusicVolume(float value)
    {
        _musicVolume = value;
        ApplyMusicVolume();
    }

    public void SetSFXVolume(float value)
    {
        _sfxVolume = value;
        ApplySFXVolume();
    }

    void ApplyMusicVolume()
    {
        if (GameManager.Instance == null || GameManager.Instance.music == null) return;

        float vol = _isMusicOn ? _musicVolume : 0f;
        GameManager.Instance.music.volume = vol;
    }

    void ApplySFXVolume()
    {
        float vol = _isSfxOn ? _sfxVolume : 0f;

        AudioListener.volume = vol;
    }

    // ================= GAME =================

    public void ResetGame()
    {
        GameManager.Instance.ResetGame();
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}