using System;
using UnityEngine;

public class PlayerEvents
{
    public event Action onRefillIncense;
    public void RefilIncense()
    {
        onRefillIncense?.Invoke();
    }

    public event Action onProgessLoop;
    public void ProgressLoop()
    {
        onProgessLoop?.Invoke();
    }

    public event Action onCompleteInteract;
    public void CompleteInteract()
    {
        onCompleteInteract?.Invoke();
    }

    public event Action<AreaEnum> onMoveToArea;
    public void MoveToArea(AreaEnum area)
    {
        onMoveToArea?.Invoke(area);
    }

    public event Action<bool> onEnableMovement;
    public void EnableMovement(bool value)
    {
        onEnableMovement?.Invoke(value);
    }

    public event Action<bool> onEnableInteract;
    public void EnableInteract(bool value)
    {
        onEnableInteract?.Invoke(value);
    }

    public event Action onRespawnPlayer;
    public void RespawnPlayer()
    {
        onRespawnPlayer?.Invoke();
    }

    public event Action onStartTurning;
    public void Startturning()
    {
        onStartTurning?.Invoke();
    }
}
