using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public AudioSource music;

    private double _startDspTime;
    private double _pausedTime;
    private bool _isPaused;

    public double StartDspTime => _startDspTime;
    public bool IsPaused => _isPaused;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartGame()
    {
        _startDspTime = AudioSettings.dspTime + 0.1;
        music.PlayScheduled(_startDspTime);

        _isPaused = false;
    }

    public void PauseGame()
    {
        if (_isPaused) return;

        _pausedTime = AudioSettings.dspTime - _startDspTime;

        music.Pause();
        Time.timeScale = 0;

        _isPaused = true;
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;

        _startDspTime = AudioSettings.dspTime - _pausedTime;

        music.UnPause();
        Time.timeScale = 1;

        _isPaused = false;
    }

    public void ResetGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}