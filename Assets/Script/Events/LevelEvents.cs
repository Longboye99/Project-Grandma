using System;
using UnityEngine;

public class LevelEvents
{
    public event Action<Interactable, AreaEnum> onTriggerInteractable;
    public void TriggerInteractable(Interactable interactable, AreaEnum area)
    {
        onTriggerInteractable?.Invoke(interactable, area);
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
