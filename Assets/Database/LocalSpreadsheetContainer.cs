using Game.Database;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "LocalSpreadsheetContainer/LocalSpreadsheetContainer", order = 1)]
public class LocalSpreadsheetContainer : ScriptableObject
{
    public int level;
    [SerializeField] LocalSpreadsheetContent content;
    public LocalSpreadsheetContent Content => content;

}
