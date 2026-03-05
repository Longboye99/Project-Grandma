using Game.Database;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "LocalSpreadsheetContainer/LocalSpreadsheetContainer", order = 1)]
public class LocalSpreadsheetContainer : ScriptableObject
{
    public int level;
    public string cutsceneLevel;
    public bool skipCutscene;
    public bool skipTutorial;
    public bool playEndingSequence;
    [SerializeField] LocalSpreadsheetContent content;
    public LocalSpreadsheetContent Content => content;

}
