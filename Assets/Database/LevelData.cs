using UnityEngine;
using System.Collections.Generic;

namespace Game.Database
{
    [System.Serializable]
    public class LevelData
    {
        public string Hour;
        public float Time;
        public int Difficulty;
        public float ActiveInterval;
        public int LightAnomalyThreshold;
        public int HeavyAnomalyThreshold;
        public float IncenseDrainSpeed;
        public float TimeScale;
    }
}

