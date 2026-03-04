using System.Collections;
using UnityEngine;

public class EndingCutscene : MonoBehaviour
{
    [SerializeField] bool enableFinalSequence;
    [SerializeField] DirtPatchInteractable dirtPatchInteract;
    [SerializeField] TestEnemy2 enemy;
    [SerializeField] BloodPoolSpawn bloodPoolSpawn;
    [SerializeField] IncensePulsingWarner incenseWarner;
    [SerializeField] int anomalyPoint;
    [SerializeField] int tempAnomalyThreshold;

    bool warnedSecondTime = false;
    bool clearedAnomaly = false;
    bool startedHaywire = false;
    bool completedEndingRequirement = false;

    private void OnEnable()
    {
        GameEventsManager.instance.levelEvents.onTriggerInteractable += OnInteractDirtPatch;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += OnUndoAnomaly;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.levelEvents.onTriggerInteractable -= OnInteractDirtPatch;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= OnUndoAnomaly;

    }


    private void Update()
    {
        float currentTime = GameManager.instance.levelManager.currentTime;
        if (!enableFinalSequence) //if not activate the dirt patch yet
        {          
            if (currentTime > 230)
            {
                enableFinalSequence = true;
                FirstWarning();
            }
        }
        else if (enableFinalSequence && !completedEndingRequirement) //activated dirt patch
        {
            anomalyPoint = GameManager.instance.anomalyManager.TallyAnomalyPoint();

            if (currentTime >= 270 && !warnedSecondTime) //warn player 1 more time in case missed clue
            {
                warnedSecondTime = true;
                SecondWarning();
            }  
            else if (!clearedAnomaly && currentTime > 300) //if done nothing til time's up
            {
                if(startedHaywire == false)
                {
                    startedHaywire = true;
                    BadEndScene();
                }
                if ( anomalyPoint >= tempAnomalyThreshold )
                {
                    //death
                    Debug.LogWarning("You're going to have a bad time");
                    completedEndingRequirement = true;

                }
            }
            else if (clearedAnomaly && anomalyPoint >= tempAnomalyThreshold) //if undo the anomaly and die
            {
                Debug.LogWarning("You died, nvm you lived");
                GoodEndScene();
                completedEndingRequirement = true;

            }
            else if (clearedAnomaly && currentTime >= 360)//if undo the anomaly and lives
            {
                Debug.LogWarning("True Ending");
                TrueEndingScene();
                completedEndingRequirement = true;

            }
        }
    }
    private void FirstWarning()
    {
        GameManager.instance.uiManager.subtitleTextController.SetSubtitleText("ไปที่ห้องน้ำ //ยาย.text", 7);
        dirtPatchInteract.EnableDirtPatch();
    }

    private void SecondWarning()
    {
        GameManager.instance.uiManager.subtitleTextController.SetSubtitleText("ไปที่ห้องน้ำ //ยาย.text", 7);
        StartCoroutine(LowRumbling());
    }

    IEnumerator LowRumbling()
    {
        GameManager.instance.uiManager.screenShake.StartLongShake();
        yield return new WaitForSeconds(3);
        GameManager.instance.uiManager.screenShake.StopLongScrenShake();
    }

    private void OnInteractDirtPatch(Interactable interactable, AreaEnum area, InteractMode mode)
    {
        if(interactable == dirtPatchInteract)
        {
            //do animation
        }
    }

    private void OnUndoAnomaly(Anomaly anomaly) //hijack normal defeat check once player undo the anomaly to start the haywire
    {
        if(anomaly == dirtPatchInteract.anomalyObject)
        {
            GameManager.instance.anomalyManager.dictionary.lockEnemyUpdate = true;
            GameManager.instance.levelManager.checkVictoryDefeat = false;
            PlayHaywireAnimation();
            clearedAnomaly = true;
        }
    }

    private void PlayHaywireAnimation()
    {
        //animation
        BeginNormalHaywirePhase();
    }

    private void BeginNormalHaywirePhase()
    {
        GameManager.instance.anomalyManager.dictionary.EnableAllAnomaly();
        enemy.cooldownDuration = 1.5f;
        enemy.difficultyLevel = 20;
        enemy.lightAnomalyThreshold = 0;
        enemy.heavyAnomalyThreashold = 9000;
        incenseWarner.isHaywireMode = true;
        GameManager.instance.anomalyManager.isHaywire = true;
    }
    private void BeginExtremeHaywirePhase()
    {
        GameManager.instance.anomalyManager.dictionary.EnableAllAnomaly();
        enemy.cooldownDuration = 0.5f;
        enemy.difficultyLevel = 20;
        enemy.lightAnomalyThreshold = 0;
        incenseWarner.isHaywireMode = true;
        GameManager.instance.anomalyManager.isHaywire = true;

    }

    public void BadEndScene()
    {
        //Shriek
        //GameManager.instance.sfxManager.PlaySoundFXClip();
        //shake,flicker
        GameManager.instance.uiManager.screenShake.StartLongShake();
        //break bracelet
        GameManager.instance.jumpscareManager.anomalyWarner.PlayBraceletBreakAnimation();
        //spawn blood pools
        bloodPoolSpawn.SpawnBloodPools(1, 2.5f, 0.4f);
        //heavy haywire
        BeginExtremeHaywirePhase();
    }

    private void GoodEndScene()
    {
        //good cutscene
        //haywire
        //fake death scene
        //end
    }

    private void TrueEndingScene()
    {
        //basically true ending but didnt die
    }

    /*
    Check time 3:50
    Spawn anomaly
    trutorial text pop up

    if UndoAnomaly that anomaly
    >trigger final boss sequence

    do another warning at 4:30
    
    else if wait til 5 am
    >trigger final boss sequence but bad
     
     
    final sequence
    jumpscare
    everything haywire
    bump up anomaly spawn rate
    
    reach max anomaly
    else if bad ver, die
    if normal ver, do victory timeline
    
     
     */
}
