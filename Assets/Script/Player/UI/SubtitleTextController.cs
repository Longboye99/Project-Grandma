using UnityEngine;
using TMPro;

public class SubtitleTextController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI subtitleDisplay;
    [SerializeField] Animator textAnimator;

    private void Start()
    {
        subtitleDisplay.gameObject.SetActive(false);
    }

    public void SetSubtitleText(string text, float sec)
    {
        subtitleDisplay.text = text;
        subtitleDisplay.gameObject.SetActive(true);

        Invoke("DisableTitleText", sec);
    }

    public void SetSubtitleText(string text)
    {
        subtitleDisplay.text = text;
        subtitleDisplay.gameObject.SetActive(true);
    }

    public void DisableTitleText()
    {
        subtitleDisplay.gameObject.SetActive(false);
    }
}
