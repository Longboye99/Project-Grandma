using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class DirtPatchInteractable : Interactable
{
    [SerializeField] bool enabledDirtPatch = false;
    public Anomaly anomalyObject;

    private void Start()
    {
        useOutline = false;
        GetComponent<Collider>().enabled = false;
    }

    public void EnableDirtPatch()
    {
        enabledDirtPatch = true;
        useOutline = true;
        GetComponent<Collider>().enabled = true;

    }

    public override void Interact(Interactable interactable, AreaEnum area, InteractMode mode)
    {
        if (interactable == this && mode == InteractMode.Hold && enabledDirtPatch)
        {
            TriggerAnomaly();
        }
    }

    private void TriggerAnomaly()
    {
        GameManager.instance.anomalyManager.TriggerAnomaly(anomalyObject);
        this.GetComponent<Collider>().enabled = false;
    }

}
