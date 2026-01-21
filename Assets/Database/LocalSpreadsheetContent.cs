using Game.Database;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalSpreadsheetContent
{
    public List<AnomalyData> anomalies;
    public List<LevelData> levelConfigs;
    public List<LevelAnomalyData> AnomalyConfig;
}
