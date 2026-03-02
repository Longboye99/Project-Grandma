using System;
using UnityEngine;

public class LevelEvents
{
    public event Action<Interactable, AreaEnum, InteractMode> onTriggerInteractable;
    public void TriggerInteractable(Interactable interactable, AreaEnum area, InteractMode mode)
    {
        onTriggerInteractable?.Invoke(interactable, area, mode);
    }

    public event Action onPlayerVictory;
    public void PlayerVictory()
    {
        onPlayerVictory?.Invoke();
    }

    public event Action onPlayerDefeated;
    public void PlayerDefeated()
    {
        onPlayerDefeated?.Invoke();
    }
}

public enum InteractMode
{
    Click,
    Hold
}
