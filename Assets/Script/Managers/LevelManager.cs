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

    public Canvas VictoryMessage;

    bool isDefeated;
    bool litIncense;
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
        currentLoop = 0;
        incenseCurrentTime = 40;
        SetIncenseSize();
        //incenseCurrentTime = incenseMaxTime;
    }


    public void ProgessLoop() // call when go to sleep or die
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
            currentLoop--;
            if(currentLoop < 0)
            {
                currentLoop = 0;
            }
        }
        GameManager.instance.anomalyManager.SpawnNextLoopAnomaly();
        playerObject.transform.position = respawnPoint.transform.position;
        litIncense = false;

        Invoke("WakeUp", 2);
        //fade in, enable movement
        //wake up
    }

    private void WakeUp()
    {
        GameManager.instance.uiManager.TransitionIn();
        GameManager.instance.playerManager.EnablePlayerMovement();
    }

    private void Victory()
    {
        Time.timeScale = 0;
        VictoryMessage.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

    public void RefillIncense()
    {
        incenseCurrentTime = incenseMaxTime;
        litIncense = true;
        SetIncenseSize();
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
