using UnityEngine;


public class LightflickerAddOn : MonoBehaviour
{
    Anomaly anomaly;
    [SerializeField] float minTime = 8;
    [SerializeField] float maxTime = 15;
    [SerializeField] Transform lightTransform;
    [SerializeField] Animator lightAnimator;
    [SerializeField] AudioClip flickerSfx;
    [SerializeField] float volumn;

    [Header("Debug")]
    [SerializeField]float timer;
    [SerializeField]float duration;
    bool countingDown;
    bool isFirstTime = true;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += UndoAnomaly;

    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= UndoAnomaly;

    }

    void Start()
    {
        anomaly = GetComponent<Anomaly>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anomaly.isActive && !countingDown)
        {
            if (isFirstTime)
            {
                timer = 0;
                duration = 2;
                countingDown = true;
                isFirstTime = false;
            }
            else
            {
                timer = 0;
                duration = Random.Range(minTime, maxTime);
                countingDown = true;
            }
            
        }

        if (anomaly.isActive && countingDown)
        {
            timer += Time.deltaTime;
            if( timer > duration )
            {
                lightAnimator.SetTrigger("Flicker");
                GameManager.instance.sfxManager.PlaySoundFXClip(flickerSfx, lightTransform, volumn);
                countingDown = false;
            }
        }

    }

    private void UndoAnomaly(Anomaly targetAnomaly)
    {
        if (anomaly == targetAnomaly && anomaly.isActive)
        {
            countingDown = false;
        }
    }
}
