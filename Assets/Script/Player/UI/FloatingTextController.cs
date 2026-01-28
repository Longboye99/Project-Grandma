using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingTextController : MonoBehaviour
{
    [SerializeField] GameObject movementTutorial;
    [SerializeField] GameObject incenseTutorial;
    GameObject currentTutorial;
    Animator currentAnimator;

    [SerializeField] Animator textAnimator;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += RemoveTutorialText;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= RemoveTutorialText;

    }

    public void EnableTutorialText(TutorialText tutorialType, float sec)
    {
        currentTutorial = SelectText(tutorialType);
        currentAnimator = currentTutorial.GetComponent<Animator>();

        currentTutorial.SetActive(true);

        Invoke("DisableTutorialText", sec);
    }

    public void SetSubtitleText(TutorialText tutorialType)
    {
        currentTutorial = SelectText(tutorialType);
        currentAnimator = currentTutorial.GetComponent<Animator>();

        currentTutorial.SetActive(true);
    }

    public void DisableTutorialText()
    {
        currentAnimator.SetTrigger("TextFading");
    }

    private void RemoveTutorialText(string eventName)
    {
        if (eventName == "FinishTextFading")
        {
            currentTutorial.SetActive(false);
            currentAnimator = null;
        }
    }

    public GameObject SelectText(TutorialText type)
    {
        if( type == TutorialText.MovementTutorial)
        {
            return movementTutorial;
        }
        else if ( type == TutorialText.IncenseTutorial)
        {
            return incenseTutorial;
        }
        else
        {
            return null;
        }
    }
}

public enum TutorialText
{
    MovementTutorial,
    IncenseTutorial,
}
