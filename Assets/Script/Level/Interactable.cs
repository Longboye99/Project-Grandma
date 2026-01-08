using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    private void OnEnable()
    {
        GameEventsManager.instance.levelEvents.onTriggerInteractable += Interact;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.levelEvents.onTriggerInteractable -= Interact;
    }

    public abstract void Interact(Interactable interactable);

    void Update()
    {
        if( GameManager.instance.playerManager.currentInteractable == this)
        {
            this.GetComponent<Outline>().enabled = true;
        }
        else
        {
            this.GetComponent<Outline>().enabled = false;
        }
    }
}
