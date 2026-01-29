using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingTextController : MonoBehaviour
{
    [SerializeField] GameObject movementTutorial;
    [SerializeField] GameObject incenseTutorial;
    [SerializeField] GameObject anomalyTutorial;
    GameObject currentTutorial;
    Animator currentAnimator;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += RemoveTutorialText;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= RemoveTutorialText;

    }

    private void Start()
    {
        movementTutorial.SetActive(false);
        incenseTutorial.SetActive(false);
    }

    public void EnableTutorialText(TutorialText tutorialType, float sec)
    {
        currentTutorial = SelectText(tutorialType);
        currentAnimator = currentTutorial.GetComponent<Animator>();

        currentTutorial.SetActive(true);

        Invoke("DisableTutorialText", sec);
    }

    public void EnableTutorialText(TutorialText tutorialType)
    {
        currentTutorial = SelectText(tutorialType);
        currentAnimator = currentTutorial.GetComponent<Animator>();

        currentTutorial.SetActive(true);
    }

    public void DisableTutorialText()
    {
        if(currentAnimator != null)
        {
            currentAnimator.SetTrigger("TextFadeOut");
        }
    }

    public void RemoveTutorialText(string eventName)
    {
        if (eventName == "FinishTextFading")
        {
            currentTutorial.SetActive(false);
            currentTutorial = null;
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
        else if ( type == TutorialText.AnomalyTutorial )
        {
            return anomalyTutorial;
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
    AnomalyTutorial,

}
