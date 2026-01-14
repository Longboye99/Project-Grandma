using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class LevelManager : MonoBehaviour
{
    [Header("Level Config")]
    public float currentTime;
    public float midnightTime = 120;
    public float finishTime = 360;
    public float timeSpeed = 1;
    public float incenseSpeed = 1;

    [SerializeField] Transform respawnPoint;
    [SerializeField] GameObject playerObject;

    [Header("Incense Config")]
    [SerializeField] float incenseCurrentTime;
    [SerializeField] Incense incense;
    [SerializeField] float incenseMaxTime;
    [SerializeField] int incenseSection;
    [SerializeField] int maxIncenseSection;

    public Canvas VictoryMessage;
    public Canvas DefeatMessage;

    bool isDefeated;
    float size;


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

    }
    private void Update()
    {
        UpdateTime();
        SetIncenseSize();
        CheckVictory();
        CheckDefeat();

        GameManager.instance.anomalyManager.CheckEnemyEvent(currentTime);
        GameManager.instance.anomalyManager.TallyAnomalyPoint();
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

    public void RespawnPlayer()
    {
        GameManager.instance.uiManager.TransitionOut();
        GameManager.instance.anomalyManager.UndoAllAnomaly();
        Invoke("RespawnPlayer2", 2);
    }

    private void RespawnPlayer2()
    {
        GameManager.instance.playerManager.TeleportPlayerToRespawn();
        GameManager.instance.uiManager.TransitionIn();

        //fancy snap incense animation them

        SnapIncense();
        PauseTimer(false);
        GameEventsManager.instance.playerEvents.EnableMovement(true);
    }
    
    public void FinishedDefeatAnim()
    {
        DefeatMessage.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    /*public void ProgessLoop() // call when go to sleep or die
    {
        //low incense
        incenseCurrentTime = 40;
        SetIncenseSize();

        //fade out anim, disable player movement
        GameManager.instance.uiManager.TransitionOut();
        GameManager.instance.playerManager.DisablePlayerMovement();

        //Check if all anomaly is cleared
        if (GameManager.instance.anomalyManager.ActiveAnomalies.Count == 0 && litIncense == true)
        {
            currentLoop++;
            Debug.Log("Current Loop" + currentLoop);
            if (currentLoop >= maxLoop)
            {
                Invoke("Victory", 2);
                return;
            }
        }
        else //yes > +1 loop,   no > something
        {
            if(currentLoop < 0)
            {
                currentLoop = 0;
            }
        }
        
        playerObject.transform.position = respawnPoint.transform.position;
        litIncense = false;

        Invoke("WakeUp", 2);
        //fade in, enable movement
        //wake up
    }
    */
    /*
    private void WakeUp()
    {
        GameManager.instance.anomalyManager.SpawnNextLoopAnomaly();
        GameManager.instance.uiManager.TransitionIn();
        GameManager.instance.playerManager.EnablePlayerMovement();
    }
    */
    private void UpdateTime()
    { 
        currentTime += Time.deltaTime * timeSpeed;
        incenseCurrentTime -=  Time.deltaTime * incenseSpeed;
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
