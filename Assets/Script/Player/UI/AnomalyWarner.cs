using UnityEngine;

public class AnomalyWarner : MonoBehaviour
{
    [SerializeField] AudioSource anomalyNoise;
    [SerializeField] int noiseWarningPoint;
    bool _isPlayingNoise;

    [SerializeField] Animator flashlightWarningAnimator;
    [SerializeField] int flashWarningPoint;

    [SerializeField] Animator braceletAnimator;
    [SerializeField] int braceletWarningPoint;
    bool _isBlinking;
    bool braceletBroke = false;
    
    public int anomalyPoint;
    public int localAnomalyPoint;
    public int temp;

    [SerializeField] float cooldown;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onStartJumpscare += OnStartJumpscare;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onStartJumpscare -= OnStartJumpscare;

    }

    private void Start()
    {
        
    }

    public void OnStartJumpscare()
    {
        anomalyNoise.Stop();
    }

    private void Update()
    {
        HandleGlobalAnomalyWarning();

        cooldown += Time.deltaTime;
        if(cooldown >= 5)
        {
            HandleLocalAnomalyWarning();
            cooldown = 0;
        }
    }

    private void HandleGlobalAnomalyWarning()
    {
        anomalyPoint = GameManager.instance.anomalyManager.TallyAnomalyPoint();
        if (anomalyPoint >= noiseWarningPoint && !_isPlayingNoise)
        {
            anomalyNoise.Play();
            _isPlayingNoise = true;
        }
        else if (anomalyPoint < noiseWarningPoint && _isPlayingNoise)
        {
            anomalyNoise.Stop();
            _isPlayingNoise = false;
        }

        if (_isBlinking)
        {
            float speed = ((float)anomalyPoint * 0.066f) - 2.6f;
            if(speed < 0.4)
            {
                speed = 0.4f;
            }
            braceletAnimator.speed = speed;
        }

        if (anomalyPoint >= braceletWarningPoint && !_isBlinking)
        {
            braceletAnimator.SetTrigger("StartBlinking");
            _isBlinking = true;
        }
        else if (anomalyPoint < braceletWarningPoint && _isBlinking)
        {
            braceletAnimator.SetTrigger("StopBlinking");
            _isBlinking = false;
        }
    }

    private void HandleLocalAnomalyWarning()
    {
        localAnomalyPoint = GameManager.instance.anomalyManager.TallyAreaAnomalyPoint();

        if (localAnomalyPoint >= flashWarningPoint)
        {
            flashlightWarningAnimator.SetTrigger("LongBlink");
        }
        else if ( 0 < localAnomalyPoint && localAnomalyPoint < flashWarningPoint)
        {
            flashlightWarningAnimator.SetTrigger("ShortBlink");
        }
    }
    
    public void PlayBraceletBreakAnimation()
    {
        if(braceletBroke == false)
        {
            braceletBroke = true;
            braceletAnimator.SetTrigger("Break");

        }
    }

    public void SetAmbienceVolumn(float volumn)
    {
        anomalyNoise.volume = volumn;
    }
}
