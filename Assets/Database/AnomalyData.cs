using UnityEngine;
using System.Collections.Generic;

namespace Game.Database
{
    [System.Serializable]
    public class AnomalyData
    {
        public string Id;
        public int AnomalyPoint;
        public int Cooldown;
        public string Level;
    }
}

