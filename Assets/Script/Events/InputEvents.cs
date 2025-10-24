using UnityEngine;
using System;

public class InputEvents
{
    public InputEventContextEnum inputEventContext { get; private set; } = InputEventContextEnum.Default;
    public void ChangeInputeventContext(InputEventContextEnum newContext)
    {
        this.inputEventContext = newContext;
    }

    public event Action<Vector2> onMovePressed;
    public void MovePressed(Vector2 moveDir)
    {
        onMovePressed?.Invoke(moveDir);
    }

    public event Action<InputEventContextEnum> onStartInteract;
    public void StartInteract()
    {
        onStartInteract?.Invoke(this.inputEventContext);
    }

    public event Action<InputEventContextEnum> onCancelInteract;
    public void CancelInteract()
    {
        onCancelInteract?.Invoke(this.inputEventContext);
    }

    public event Action onPause;
    public void Pause()
    {
        onPause?.Invoke();
    }
}
