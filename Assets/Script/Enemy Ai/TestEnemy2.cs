using System.Collections.Generic;
using UnityEngine;

public class TestEnemy2 : MonoBehaviour
{
    [Header("Anomalies")]
    public int difficultyLevel;
    public int lightAnomalyThreshold;
    public int heavyAnomalyThreashold;
    public float cooldownDuration;
    public float graceDuration;
    [SerializeField] GameObject ghostPrefab;

    [Header("State")]
    [SerializeField] public float currentCooldown;
    [SerializeField] public float currentGrace;
    [SerializeField] private bool isAttacking;
    [SerializeField] private int anomalyPoint;
    [SerializeField] int failCount;

    private AnomalyManager anomalyManager;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += CheckFinishAttackAnomaly;
        GameEventsManager.instance.levelEvents.onPlayerDefeated += StartJumpscare;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= CheckFinishAttackAnomaly;
        GameEventsManager.instance.levelEvents.onPlayerDefeated -= StartJumpscare;

    }

    private void Start()
    {
        anomalyManager = GameManager.instance.anomalyManager;
    }
    
    private void Update()
    {
        if(currentCooldown <= 0)
        {
            TrySpawningAnomaly();
        }
        else
        {
            currentCooldown -= Time.deltaTime;
            if(currentGrace >= 0)
            {
                currentGrace -= Time.deltaTime;
            }      
        }
    }
    
    private void TrySpawningAnomaly()
    {
        anomalyPoint = 0;
        anomalyPoint = anomalyManager.TallyAnomalyPoint();
        Debug.Log("Try Spawning Anomaly At Anomaly Point: " + anomalyPoint);

        int difficultyRoll = Random.Range(0, 20);
        if (failCount > 0)
        {
            difficultyRoll -= ((int)Mathf.Floor((20 - difficultyLevel) * 0.4f));
        }
        if (failCount >= 2 )
        {
            difficultyRoll = 0;
        }

        if (difficultyLevel >= difficultyRoll && currentGrace <= 0)
        { 
            if (anomalyPoint >= heavyAnomalyThreashold)
            {
                GameEventsManager.instance.anomalyEvents.StartJumpscare();
                currentGrace = graceDuration;
            }

            else if (anomalyPoint >= lightAnomalyThreshold)
            {
                if (anomalyManager.SpawnRandomHeavyAnomaly() == false)
                {
                    if (anomalyManager.SpawnRandomLightAnomaly() == false)
                    {                       
                        failCount++;
                        Debug.Log("Failed all spawning attempt: " + failCount);
                    }
                    else
                    {
                        failCount = 0;
                    }
                }
                else
                {
                    failCount = 0;

                }
            }
            else if (anomalyManager.SpawnRandomLightAnomaly() == false)
            {
                failCount++;
                Debug.Log("Failed all spawning attempt: " + failCount);
            }
            else { failCount = 0; }
        }
        else
        {
            failCount++;
            Debug.Log("Failed all spawning attempt: " + failCount);
        }
        currentCooldown = cooldownDuration;
        anomalyPoint = anomalyManager.TallyAnomalyPoint();
    }


    private void CheckFinishAttackAnomaly(Anomaly anomaly)
    {
        if(anomaly.anomalyLevel == AnomalyEnum.AttackAnomaly)
        {
            GameEventsManager.instance.anomalyEvents.SnapIncense();
            GameManager.instance.anomalyManager.UndoAllAnomaly();
            currentGrace = graceDuration;
        }
    }

    private void StartJumpscare()
    {
        GameObject ghost = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
        ghost.GetComponent<ChaseJumpscareHandler>().StartJumpscare(3);
    }

}
