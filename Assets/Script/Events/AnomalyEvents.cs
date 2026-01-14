using UnityEngine;
using System;

public class AnomalyEvents
{
    public event Action<Anomaly> onUndoAnomaly;
    public void UndoAnomaly (Anomaly anomaly)
    {
        onUndoAnomaly?.Invoke (anomaly);
    }

    public event Action onStartHoldingAnomaly;
    public void StartHoldingAnomaly ()
    {
        onStartHoldingAnomaly?.Invoke ();
    }

    public event Action onTriggerLightAnomaly;
    public void TriggerLightAnomaly()
    {
        onTriggerLightAnomaly?.Invoke ();
    }

    public event Action onTriggerHeavyAnomaly;
    public void TriggerHeavyAnomaly()
    {
        onTriggerHeavyAnomaly?.Invoke ();
    }

    public event Action onTriggerAttackAnomaly;
    public void TriggerAttackAnomaly()
    {
        onTriggerAttackAnomaly?.Invoke ();
    }

    public event Action onSnapIncense;
    public void SnapIncense()
    {
        onSnapIncense?.Invoke ();
    }

    public event Action onStartJumpscare;
    public void StartJumpscare()
    {
        onStartJumpscare?.Invoke ();
    }

    public event Action onFinishJumpscare;
    public void FinishJumpscare()
    {
        onFinishJumpscare?.Invoke ();
    }

    public event Action<string> onFinishAnimationEvent;
    public void FinishAnimationEvent(string name)
    {
        onFinishAnimationEvent?.Invoke(name);
    }

    //-------------------------------------------------------

    public event Action onTriggerChasedAnomaly;
    public void TriggerChasedAnomaly()
    {
        onTriggerChasedAnomaly?.Invoke ();
    }

    public event Action onFinishPraying;
    public void FinishPraying()
    {
        if (onFinishPraying != null)
        {
            onFinishPraying();
        }
    }
}
