using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour
{
    [Header("Level Config")]
    public int currentLoop = -1;
    public int maxLoop = 6;
    public float currentTime;
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

    public GameObject VictoryMessage;
    public GameObject DefeatMessage;

    bool isDefeated;
    float size;


    private void OnEnable()
    {
        GameEventsManager.instance.playerEvents.onRefillIncense += RefillIncense;
        GameEventsManager.instance.playerEvents.onProgessLoop += ProgessLoop;
        GameEventsManager.instance.anomalyEvents.onSnapIncense += SnapIncense;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.playerEvents.onRefillIncense -= RefillIncense;
        GameEventsManager.instance.playerEvents.onProgessLoop -= ProgessLoop;
        GameEventsManager.instance.anomalyEvents.onSnapIncense -= SnapIncense;
    }
    private void Start()
    {
        incenseSection = maxIncenseSection;
        ProgessLoop();
        //incenseCurrentTime = incenseMaxTime;
    }

    private void Update()
    {
        SetIncenseSize();


        //GameManager.instance.anomalyManager.CheckEnemyEvent(currentTime);
    }

    public void ProgessLoop() // call when go to sleep or die
    {
        //low incense
        incenseCurrentTime = 40;
        SetIncenseSize();
        //fade out anim, disable player movement

        //Check if all anomaly is cleared
        if (GameManager.instance.anomalyManager.ActiveAnomalies.Count == 0)
        {
            currentLoop++;
            Debug.Log("Current Loop" + currentLoop);
            if (currentLoop >= maxLoop)
            {
                VictoryMessage.SetActive(true);
                Time.timeScale = 0;
            }
        }
        else //yes > +1 loop,   no > something
        {
            currentLoop--;
            if(currentLoop < 0)
            {
                currentLoop = 0;
            }
        }
        GameManager.instance.anomalyManager.SpawnNextLoopAnomaly();
        playerObject.transform.position = respawnPoint.transform.position;

        //fade in, enable movement
        //wake up
    }


    private void UpdateTime()
    { 
        currentTime += Time.deltaTime * timeSpeed;
        incenseCurrentTime -=  Time.deltaTime * incenseSpeed;
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

    private void RefillIncense()
    {
        incenseCurrentTime = incenseMaxTime;
    }

    //----------------------------------------------------------------

    /*private void CheckVictory()
{
    if (currentTime >= finishTime)
    {
        VictoryMessage.SetActive(true);
        Time.timeScale = 0;
    }
}
private void CheckDefeat()
{
    if (incenseCurrentTime <= 0 && !isDefeated)
    {
        isDefeated = true;
        timeSpeed = 0;
        GameEventsManager.instance.levelEvents.PlayerDefeated();
    }
}

public void FinishedDefeatAnim()
{
    DefeatMessage.SetActive(true);
    Time.timeScale = 0;
}
*/



}
