using UnityEngine;

public abstract class Jumpscare : MonoBehaviour
{
    
    public JumpscareType JumpscareType;
    public AreaEnum area;
    
    public abstract void TriggerJumpscare();

    public abstract void DisableJumpscare();

}

public enum JumpscareType
{
    OnScreen,
    Transition,
    AfterInteract
}

