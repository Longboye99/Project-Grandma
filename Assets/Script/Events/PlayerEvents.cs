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
}
