using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DoorJumpscare : Jumpscare
{
    [Header("Prefabs")]
    [SerializeField] GameObject ghostPrefab;
    [SerializeField] GameObject jumpscarePrefab;

    [SerializeField] Animator jumpscareAnimator;
    [SerializeField] Transform finalPos;

    Transform jumpscareCanvas;
    GameObject ghost;
    GameObject jumpscare;
    bool isActive;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += FinishAnimationEvent;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= FinishAnimationEvent;
    }

    private void Start()
    {
        jumpscareCanvas = GameObject.FindGameObjectWithTag("Jumpscare").transform;

    }

    public override void TriggerJumpscare()
    {
        ghost = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
        isActive = true;

        StartCoroutine(MoveGhost());
    }

    private IEnumerator MoveGhost()
    {
        Vector3 startingPos = ghost.transform.position;
        Vector3 endPos = finalPos.transform.position;
        float elapsedTime = 0;
        float duration = 0.3f;

        while(elapsedTime < duration)
        {
            ghost.transform.position = Vector3.Lerp(startingPos, endPos, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ghost.transform.position = finalPos.transform.position;

        StopGhost();
    }

    private void StopGhost()
    {
        StartCoroutine(WaitForSeconds());
    }

    private IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(0.2f);
        DoJumpscare();
    }

    private void DoJumpscare()
    {
        Destroy(ghost);

        jumpscare = Instantiate(jumpscarePrefab, jumpscareCanvas);
        jumpscareAnimator = jumpscare.GetComponent<Animator>();
        jumpscareAnimator.SetTrigger("TriggerJumpscare");
    }

    private void FinishAnimationEvent(string name)
    {
        if (name == "FinishJumpscare" && isActive)
        {
            DisableJumpscare();
        }
    }

    public override void DisableJumpscare()
    {
        GameEventsManager.instance.anomalyEvents.FinishJumpscare();
        isActive = false;
        Invoke("DestroyGhost", 2);
    }
    private void DestroyGhost()
    {
        Destroy(jumpscare);
        jumpscareAnimator = null;
    }
}
