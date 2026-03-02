using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected bool useOutline;
    private void OnEnable()
    {
        GameEventsManager.instance.levelEvents.onTriggerInteractable += Interact;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.levelEvents.onTriggerInteractable -= Interact;
    }

    public abstract void Interact(Interactable interactable, AreaEnum area, InteractMode mode);

    void Update()
    {
        if( GameManager.instance.playerManager.currentInteractable == this && useOutline)
        {
            this.GetComponent<Outline>().enabled = true;
        }
        else
        {
            this.GetComponent<Outline>().enabled = false;
        }
    }
}
