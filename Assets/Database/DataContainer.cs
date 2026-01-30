using NorskaLib.Spreadsheets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Database
{
    [Serializable]

    public class SpreadsheetContent
    {
        [SpreadsheetPage("Anomaly Data")]
        public List<AnomalyData> anomalies;
        [SpreadsheetPage("Night0 EnemyConfig")]
        public List<LevelData> levelConfigs;
        [SpreadsheetPage("Night0 AnomalyConfig")]
        public List<LevelAnomalyData> AnomalyConfig; 
    }

    [CreateAssetMenu(fileName = "SpreadsheetContainer", menuName = "SpreadsheetContainer")]
    public class SpreadsheetContainer : SpreadsheetsContainerBase
    {
        [SpreadsheetContent]
        [SerializeField] SpreadsheetContent content;
        public SpreadsheetContent Content => content;
    }
}
