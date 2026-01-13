using UnityEngine;

public abstract class Jumpscare : MonoBehaviour
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

