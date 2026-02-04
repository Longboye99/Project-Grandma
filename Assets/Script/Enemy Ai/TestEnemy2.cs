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
        if (difficultyLevel >= Random.Range(0, 20) && currentGrace <= 0)
        { 
            if (anomalyPoint >= heavyAnomalyThreashold)
            {
                GameEventsManager.instance.anomalyEvents.StartJumpscare();
                currentGrace = graceDuration;
            }

            else if (anomalyPoint >= lightAnomalyThreshold)
            {
                anomalyManager.SpawnRandomHeavyAnomaly();
                

            }
            else
            {
                anomalyManager.SpawnRandomLightAnomaly();
            }
        }
        else
        {
            Debug.Log("Failed Difficulty Roll");
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
