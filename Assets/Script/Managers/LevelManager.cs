using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.VisualScripting;

public class LevelManager : MonoBehaviour
{
    [Header("Level Config")]
    public float currentTime;
    public float midnightTime = 120;
    public float finishTime = 360;
    public float timeSpeed = 1;
    public float incenseSpeed = 1;


    [Header("Incense Config")]
    [SerializeField] public float incenseCurrentTime;
    [SerializeField] Incense incense;
    [SerializeField] float incenseMaxTime;
    [SerializeField] int incenseSection;
    [SerializeField] int maxIncenseSection;
    [SerializeField] GameObject incenseWarning;
    [SerializeField] float incenseWarningThreshold;
    bool isWarning;

    public Canvas VictoryMessage;
    public Canvas DefeatMessage;

    bool isDefeated;
    float size;
    PlayerCutsceneController playerCutsceneController;


    private void OnEnable()
    {
        GameEventsManager.instance.playerEvents.onRefillIncense += RefillIncense;
        GameEventsManager.instance.anomalyEvents.onSnapIncense += SnapIncense;
        GameEventsManager.instance.playerEvents.onRespawnPlayer += RespawnPlayer;

    }

    private void OnDisable()
    {
        GameEventsManager.instance.playerEvents.onRefillIncense -= RefillIncense;
        GameEventsManager.instance.anomalyEvents.onSnapIncense -= SnapIncense;
        GameEventsManager.instance.playerEvents.onRespawnPlayer -= RespawnPlayer;

    }
    private void Start()
    {
        incenseSection = maxIncenseSection;
        playerCutsceneController = GameObject.FindGameObjectWithTag("PlayerCollider").GetComponent<PlayerCutsceneController>();
        RefillIncense();
    }
    private void Update()
    {
        UpdateTime();
        SetIncenseSize();
        CheckVictory();
        CheckDefeat();
        CheckIncenseWarning();

        GameManager.instance.anomalyManager.CheckEnemyEvent(currentTime);
        GameManager.instance.anomalyManager.TallyAnomalyPoint();
    }

    private void UpdateTime()
    {
        currentTime += Time.deltaTime * timeSpeed;
        incenseCurrentTime -= Time.deltaTime * incenseSpeed;
    }

    private void CheckVictory()
    {
        if (currentTime >= finishTime)
        {
            Time.timeScale = 0;
            VictoryMessage.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
    private void CheckDefeat()
    {
        if (incenseCurrentTime <= 0 && !isDefeated)
        {
            isDefeated = true;
            Time.timeScale = 0;
            DefeatMessage.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void FinishedDefeatAnim()
    {
        DefeatMessage.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    public void PauseTimer(bool pause)
    {
        if (pause)
        {
            timeSpeed = 0;
            incenseSpeed = 0;
        }
        else
        {
            timeSpeed = 1;
            incenseSpeed = 1;
        }
    }

    private void CheckIncenseWarning()
    {
        if(incenseCurrentTime < incenseWarningThreshold && !isWarning)
        {
            Debug.LogWarning("Incense Low");
            isWarning = true;
            incenseWarning.SetActive(true);
        }
        else if(incenseCurrentTime > incenseWarningThreshold && isWarning)
        {
            isWarning = false;
            incenseWarning.SetActive(false);
        }
    }

    //-------------------------Respawn--------------------------------

    public void RespawnPlayer()
    {
        GameManager.instance.uiManager.FlashlightHand(false);
        GameManager.instance.uiManager.TransitionOut();
        GameManager.instance.anomalyManager.UndoAllAnomaly();
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(2f);

        GameManager.instance.playerManager.TeleportPlayerToRespawn();
        GameManager.instance.uiManager.TransitionIn();

        yield return new WaitForSeconds(0.2f);

        playerCutsceneController.IncenseCutsceneSequence();
    }

    public void FinishRespawnCutscene()
    {
        GameManager.instance.uiManager.FlashlightHand(true);

        PauseTimer(false);
        GameEventsManager.instance.playerEvents.EnableMovement(true);
    }
    
    //-------------------------------------------------------------------

    

    //---------------------Incense functions----------------------------
    private void SetIncenseSize()
    {
        float incensePercentage =  incenseCurrentTime / 100;
        incense.incensePercentage = incensePercentage;
    }

    private void SnapIncense()
    {
        incenseSection--;
        SetIncenseSection();
    }

    private void SetIncenseSection()
    {
        size = (float)incenseSection / maxIncenseSection;
        incenseMaxTime = incenseMaxTime * size;
        if (incenseCurrentTime > incenseMaxTime)
        {
            incenseCurrentTime = incenseMaxTime;
        }
        Debug.Log(incenseCurrentTime + "-" + incenseMaxTime);
    }

    public void RefillIncense()
    {
        incenseCurrentTime = incenseMaxTime;
        SetIncenseSize();
    }

    //----------------------------------------------------------------

    




}
