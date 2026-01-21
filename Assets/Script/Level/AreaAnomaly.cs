using UnityEngine;
using System.Collections.Generic;

public class AreaAnomaly
{
    public AreaEnum areaEnum;

    public List<Anomaly> lightAnomalies = new List<Anomaly>();
    public List<Anomaly> heavyAnomalies = new List<Anomaly>();
    public List<Anomaly> attackAnomalies = new List<Anomaly>();

    public List<Anomaly> DisabledAnomalies = new List<Anomaly>();
}
