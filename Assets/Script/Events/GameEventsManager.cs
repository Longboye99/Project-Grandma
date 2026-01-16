using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public AnomalyEvents anomalyEvents;
    public DebugEvents debugEvents;
    public PlayerEvents playerEvents;
    public LevelEvents levelEvents;
    public InputEvents inputEvents;

    private static GameEventsManager _instance;
    public static GameEventsManager instance 
    {
        get
        {
            if(!_instance)
            {
                _instance = new GameObject().AddComponent<GameEventsManager>();
                _instance.name = _instance.GetType().ToString();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    

    private void Awake()
    {
        anomalyEvents = new AnomalyEvents();
        debugEvents = new DebugEvents();    
        playerEvents = new PlayerEvents();
        levelEvents = new LevelEvents();
        inputEvents = new InputEvents();
    }
}
