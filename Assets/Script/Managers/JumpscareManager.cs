using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal.Internal;

public class JumpscareManager : MonoBehaviour
{
    /*
    on screen
    next transition
    after interact

    world space part
    ui sprite part

    enable js
    see what's the next player action

    if nothing > lock transition > js
    if turn > js upon switching scene
    if interact > js after interact

    wait for trigger
    wait til complete
    exit jumpscare
     */
    [SerializeField] AudioClip riserAudio;
    [SerializeField] float riserVolume;
    [SerializeField] List<JumpscareContainer> jumpscareList;
    Dictionary<AreaEnum, JumpscareContainer> jumpscareDict = new Dictionary<AreaEnum, JumpscareContainer>();
    PointClickCameraMovement cameraMovement;
    bool jumpscareEnabled;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onStartJumpscare += EnableJumpscare;
        GameEventsManager.instance.anomalyEvents.onFinishJumpscare += ExitJumpscare;
        GameEventsManager.instance.playerEvents.onMoveToArea += TriggerTransitionJumpscare;
        GameEventsManager.instance.levelEvents.onTriggerInteractable += TriggerInteractableJumpscare;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onStartJumpscare -= EnableJumpscare;
        GameEventsManager.instance.anomalyEvents.onFinishJumpscare -= ExitJumpscare;
        GameEventsManager.instance.playerEvents.onMoveToArea -= TriggerTransitionJumpscare;
        GameEventsManager.instance.levelEvents.onTriggerInteractable -= TriggerInteractableJumpscare;
    }

    private void Start()
    {
        cameraMovement = GameObject.FindGameObjectWithTag("PlayerCollider").GetComponent<PointClickCameraMovement>();
        CreateJumpscareDict();
    }

    private void Update()
    {

    }

    public void EnableJumpscare()
    {
        jumpscareEnabled = true;
        GameManager.instance.sfxManager.PlaySoundFXClip(riserAudio, cameraMovement.transform, riserVolume);
        StartCoroutine(StartJumpscareCountDown());
        //Start timer
        //if player do something activate instantly
        //
        //Select jumpscare logic
    }

    private IEnumerator StartJumpscareCountDown()
    {
        float duration = 7;
        

        yield return new WaitForSeconds(duration);

        if(jumpscareEnabled )
        {
            AreaEnum currentArea = cameraMovement.currentNode.area;
            TriggerStationaryJumpscare(currentArea);
        }
    }
    private void TriggerStationaryJumpscare(AreaEnum area)
    {
        if (jumpscareDict[area].stationaryJumpscare != null && jumpscareEnabled)
        {
            GameManager.instance.levelManager.timeSpeed = 0;
            jumpscareEnabled = false;
            GameEventsManager.instance.playerEvents.EnableMovement(false);
            jumpscareDict[area].stationaryJumpscare.TriggerJumpscare();
        }
    }


    private void TriggerTransitionJumpscare(AreaEnum area)
    {
        if (jumpscareDict[area].transitionJumpscare != null && jumpscareEnabled)
        {
            GameManager.instance.levelManager.timeSpeed = 0;
            jumpscareEnabled = false;
            GameEventsManager.instance.playerEvents.EnableMovement(false);
            jumpscareDict[area].transitionJumpscare.TriggerJumpscare();
        }

        //check if that area has on switch anomaly
        //do if yes
    }

    private void TriggerInteractableJumpscare(Interactable interactable, AreaEnum area)
    {
        if (jumpscareDict[area].interactJumpscare != null && jumpscareEnabled)
        {
            GameManager.instance.levelManager.PauseTimer(true);
            jumpscareEnabled = false;
            GameEventsManager.instance.playerEvents.EnableMovement(false);
            jumpscareDict[area].interactJumpscare.TriggerJumpscare();

        }
    }

    public void ExitJumpscare()
    {
        jumpscareEnabled = false;
        GameManager.instance.levelManager.JumpscareDefeat();
    }

    private void CreateJumpscareDict()
    {
        foreach (var container in jumpscareList)
        {
            jumpscareDict.Add(container.area, container);
        }
    }
}
