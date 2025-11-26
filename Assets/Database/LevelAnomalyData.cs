using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Game.Database
{
    [System.Serializable]
    public class LevelAnomalyData
    {
        public string AnomalyId;
        public string Am0;
        public string Am1;
        public string Am2;
        public string Am3;
        public string Am4;
        public string Am5;

        public List<string> activationTimes;

        public void CreateList()
        {
            activationTimes.Clear();
            activationTimes.Add(Am0);
            activationTimes.Add(Am1);
            activationTimes.Add(Am2);
            activationTimes.Add(Am3);
            activationTimes.Add(Am4);
            activationTimes.Add(Am5);
        }
    }
}

