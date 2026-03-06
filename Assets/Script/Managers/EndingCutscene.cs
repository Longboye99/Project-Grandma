using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class EndingCutscene : MonoBehaviour
{
    [SerializeField] DataSwitchContainer dataSwitcher;
    [SerializeField] LocalSpreadsheetContainer DataContainer;

    [SerializeField] bool enableFinalSequence;
    [SerializeField] DirtPatchInteractable dirtPatchInteract;
    [SerializeField] TestEnemy2 enemy;
    [SerializeField] BloodPoolSpawn bloodPoolSpawn;
    [SerializeField] IncensePulsingWarner incenseWarner;
    [SerializeField] int anomalyPoint;
    [SerializeField] int tempAnomalyThreshold;

    [Header("Morning Scene")]
    [SerializeField] Material morningSkybox;
    [SerializeField] GameObject directionaLight;
    [SerializeField] RawImage renderTexture;
    [SerializeField] Material retroEffectMorning;
    [SerializeField] PlayableDirector cutscenePlayer;


    bool warnedSecondTime = false;
    bool clearedAnomaly = false;
    bool startedHaywire = false;
    bool completedEndingRequirement = false;

    private void OnEnable()
    {
        GameEventsManager.instance.levelEvents.onTriggerInteractable += OnInteractDirtPatch;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += OnUndoAnomaly;
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += FinishAnimationEvent;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.levelEvents.onTriggerInteractable -= OnInteractDirtPatch;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= OnUndoAnomaly;
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= FinishAnimationEvent;

    }

    private void Start()
    {
        int _currentLevel = PlayerPrefs.GetInt("currentLevel");
        DataContainer = dataSwitcher.levelsData[_currentLevel];
        enableFinalSequence = DataContainer.playEndingSequence;

        if(!enableFinalSequence)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            enableFinalSequence = false;
        }
    }


    private void Update()
    {
        float currentTime = GameManager.instance.levelManager.currentTime;
        anomalyPoint = GameManager.instance.anomalyManager.TallyAnomalyPoint();

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

            if (!clearedAnomaly && currentTime >= 270 && !warnedSecondTime) //warn player 1 more time in case missed clue
            {
                warnedSecondTime = true;
                SecondWarning();
            }  
            else if (!clearedAnomaly && currentTime > 300) //if done nothing til time's up
            {
                if(startedHaywire == false)
                {
                    startedHaywire = true;
                    StartBadEndingSequence();
                }
                if ( anomalyPoint >= tempAnomalyThreshold )
                {
                    //death
                    Debug.LogWarning("You're going to have a bad time");
                    completedEndingRequirement = true;

                }
            }

            if( clearedAnomaly && startedHaywire == false && anomalyPoint >= 60)
            {
                startedHaywire = true;
                BeginExtremeHaywirePhase();
            }
            else if (clearedAnomaly && anomalyPoint >= tempAnomalyThreshold) //if undo the anomaly and die, play fakeout death cutscene
            {
                Debug.LogWarning("You died, nvm you lived");
                GameEventsManager.instance.anomalyEvents.StartJumpscare();
                GameManager.instance.jumpscareManager.fakeOut = true;
                completedEndingRequirement = true;
            }
            
            
            if (clearedAnomaly && currentTime >= 360)//if undo the anomaly and lives
            {
                Debug.LogWarning("True Ending");
                PlayTrueEndingScene();
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
            //ominoius noise/animation
        }
    }

    private void OnUndoAnomaly(Anomaly anomaly) //hijack normal defeat check once player undo the anomaly to start the haywire
    {
        if(anomaly == dirtPatchInteract.anomalyObject)
        {
            GameManager.instance.anomalyManager.dictionary.lockEnemyUpdate = true;
            GameManager.instance.levelManager.checkVictoryDefeat = false;
            StartFakeOutGoodEndingSequence();
            clearedAnomaly = true;
        }
    }

    private void BeginNormalHaywirePhase()
    {
        GameManager.instance.anomalyManager.dictionary.EnableAllAnomaly();
        enemy.cooldownDuration = 5f;
        enemy.difficultyLevel = 20;
        enemy.lightAnomalyThreshold = 0;
        enemy.heavyAnomalyThreashold = 9000;
        incenseWarner.isHaywireMode = true;
        GameManager.instance.anomalyManager.isHaywire = true;
        Debug.LogWarning("NormalHaywire");

    }
    private void BeginExtremeHaywirePhase()
    {
        GameManager.instance.anomalyManager.dictionary.EnableAllAnomaly();
        enemy.cooldownDuration = 0.5f;
        enemy.difficultyLevel = 20;
        enemy.lightAnomalyThreshold = 0;
        GameManager.instance.anomalyManager.isHaywire = true;
        Debug.LogWarning("ExtremeHaywire");

    }

    public void StartBadEndingSequence()
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

    private void StartFakeOutGoodEndingSequence()
    {
        //Shriek
        //GameManager.instance.sfxManager.PlaySoundFXClip();
        //shake,flicker
        GameManager.instance.uiManager.screenShake.StartLongShake();
        //spawn blood pools
        bloodPoolSpawn.SpawnBloodPools(2, 1.5f, 0.4f);
        //heavy haywire
        BeginNormalHaywirePhase();
        //fake death scene
        //end
    }

    public void PlayGoodEndingCutscene() //fakeout cutscene when player died after completed good ending requirement
    {
        GameManager.instance.levelManager.timeSpeed = 0;
        GameManager.instance.levelManager.incenseSpeed = 0;
        enemy.cooldownDuration = 9000000;
        incenseWarner.isHaywireMode = false;
        enemy.gameObject.SetActive(false);
        GameManager.instance.anomalyManager.UndoAllAnomaly();
        GameManager.instance.anomalyManager.DisableAllAnomaly();
        

        RenderSettings.skybox = morningSkybox;
        directionaLight.SetActive(true);
        renderTexture.material = retroEffectMorning;
        bloodPoolSpawn.RemoveBloodPool();
        GameManager.instance.uiManager.screenShake.StopLongScrenShake();

        GameManager.instance.uiManager.TransitionOut();
        Debug.LogWarning("Fakeouted");
        ContinueTimeLine();
    }

    public void ContinueTimeLine()
    {
        cutscenePlayer.Play();
    }

    private void FinishAnimationEvent(string eventName)
    {
        if (eventName == "FinishFakeOutCutscene")
        {
            GameManager.instance.uiManager.TransitionIn();
            GameManager.instance.levelManager.Victory();
            Invoke("StopCutscene", 1);
        }
    }

    private void PlayTrueEndingScene()
    {
        //basically normal ending but morning
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
