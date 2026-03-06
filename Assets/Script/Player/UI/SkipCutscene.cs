using UnityEngine;
using UnityEngine.Playables;

public class SkipCutscene : MonoBehaviour
{
    [SerializeField] PlayableDirector cutscene;
    [SerializeField] GameObject loadLevelObject;

    public void SkipCutscenePlayer()
    {
        cutscene.Stop();
        loadLevelObject.SetActive(true);
    }
}
