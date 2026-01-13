using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class JumpscareManager : MonoBehaviour
{
    [SerializeField] List<Jumpscare> jumpscares = new List<Jumpscare>();

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onStartJumpscare += EnableJumpscare;
        GameEventsManager.instance.anomalyEvents.onFinishJumpscare += ExitJumpscare;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onStartJumpscare -= EnableJumpscare;
        GameEventsManager.instance.anomalyEvents.onFinishJumpscare -= ExitJumpscare;

    }

    private void Update()
    {

    }

    public void EnableJumpscare(AreaEnum currentArea)
    {
        //Select jumpscare logic
    }

    public void ExitJumpscare()
    {
        GameManager.instance.levelManager.RespawnPlayer();
    }
}
