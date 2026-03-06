using UnityEngine;

public class UpsideDownJumpscare : Jumpscare
{
    bool isActive;
    [SerializeField] Animator ghostAnimator;


    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += FinishAnimationEvent;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= FinishAnimationEvent;
    }

    private void Start()
    {
        ghostAnimator.gameObject.SetActive(false);
    }

    public override void TriggerJumpscare()
    {
        isActive = true;
        ghostAnimator.gameObject.SetActive(true);
        GameManager.instance.uiManager.EnableGameOverlay(false);
    }

    public override void DisableJumpscare()
    {
        GameEventsManager.instance.anomalyEvents.FinishJumpscare();
        isActive = false;
        ghostAnimator.gameObject.SetActive(false);
    }

    private void FinishAnimationEvent(string name)
    {
        if (name == "FinishJumpscare" && isActive)
        {
            GameManager.instance.uiManager.BlackScreen();
            Invoke("DisableJumpscare", 3f);
        }
    }
}
