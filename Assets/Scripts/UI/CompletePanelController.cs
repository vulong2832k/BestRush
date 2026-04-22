using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class CompletePanelController : MonoBehaviour
{
    [Header("UI")]
    public RectTransform panel;
    public CanvasGroup canvasGroup;
    public Image progressBar;
    public Image rankImage;

    public TMP_Text percentText;
    public TMP_Text missText;

    [Header("Audio")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip _missionCompleteClip;

    [Header("Rank Sprites")]
    [SerializeField] private Sprite Fail;
    [SerializeField] private Sprite Normal;
    [SerializeField] private Sprite Master;
    [SerializeField] private Sprite bronze;
    [SerializeField] private Sprite silver;
    [SerializeField] private Sprite gold;

    private Vector2 _startPos;
    private Vector2 _targetPos;

    private void Awake()
    {
        _targetPos = panel.anchoredPosition;
        _startPos = _targetPos + new Vector2(0, 800);

        panel.anchoredPosition = _startPos;

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowResult(float percent, int miss)
    {
        StartCoroutine(PlaySequence(percent, miss));
    }

    private System.Collections.IEnumerator PlaySequence(float percent, int miss)
    {
        if (GameManager.Instance != null && GameManager.Instance.music != null)
        {
            GameManager.Instance.music.Stop();
        }

        float waitTime = 1f;

        if (_sfxSource != null && _missionCompleteClip != null)
        {
            _sfxSource.PlayOneShot(_missionCompleteClip);
            waitTime = _missionCompleteClip.length;
        }

        yield return new WaitForSecondsRealtime(waitTime);

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.3f).SetUpdate(true);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        panel.DOAnchorPos(_targetPos, 0.5f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        yield return new WaitForSecondsRealtime(0.5f);

        progressBar.fillAmount = 0f;
        progressBar.DOFillAmount(percent, 1f).SetUpdate(true);

        yield return new WaitForSecondsRealtime(1f);

        rankImage.sprite = GetRank(percent);

        percentText.text = $"Complete Percent: {(percent * 100f):0}%";
        missText.text = $"Number Of Missed: {miss}";
    }

    private Sprite GetRank(float percent)
    {
        if (percent >= 1f) return gold;
        if (percent >= 0.8f) return silver;
        if (percent >= 0.6f) return bronze;
        if (percent >= 0.4f) return Master;
        if (percent >= 0.2f) return Normal;

        return Fail;
    }
}