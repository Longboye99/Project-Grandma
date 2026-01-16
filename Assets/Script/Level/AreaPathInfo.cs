using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class AreaPathInfo
{
    public AreaNode areaNode;
    public SplineContainer splineContainer;
    public float duration;
    public Direction direction;
}
