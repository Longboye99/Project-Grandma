using UnityEngine;
using System.Collections.Generic;

public class AreaAnomaly
{
    public AreaEnum areaEnum;
    public int activeAnomaly;
    public List<Anomaly> lightAnomalies = new List<Anomaly>();
    public List<Anomaly> heavyAnomalies = new List<Anomaly>();
    public List<Anomaly> attackAnomalies = new List<Anomaly>();
}
