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
        public List<LevelData> nightOneLevelConfigs;
        [SpreadsheetPage("Night0 AnomalyConfig")]
        public List<LevelAnomalyData> nightOneAnomalyConfig;

        [SpreadsheetPage("Night1 EnemyConfig")]
        public List<LevelData> nightTwolevelConfigs;
        [SpreadsheetPage("Night1 AnomalyConfig")]
        public List<LevelAnomalyData> nighTwoAnomalyConfig;

        [SpreadsheetPage("Night2 EnemyConfig")]
        public List<LevelData> nightThreelevelConfigs;
        [SpreadsheetPage("Night2 AnomalyConfig")]
        public List<LevelAnomalyData> nightThreeAnomalyConfig;
    }

    [CreateAssetMenu(fileName = "SpreadsheetContainer", menuName = "SpreadsheetContainer")]
    public class SpreadsheetContainer : SpreadsheetsContainerBase
    {
        [SpreadsheetContent]
        [SerializeField] SpreadsheetContent content;
        public SpreadsheetContent Content => content;
    }
}
