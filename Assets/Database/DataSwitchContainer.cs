using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class DataSwitchContainer : ScriptableObject
{
    public LocalSpreadsheetContainer currentData;

    public List<LocalSpreadsheetContainer> levelsData;
}
