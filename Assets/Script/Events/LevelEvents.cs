using System;
using UnityEngine;

public class LevelEvents
{
    public event Action<Interactable> onTriggerInteractable;
    public void TriggerInteractable(Interactable interactable)
    {
        onTriggerInteractable?.Invoke(interactable);
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
