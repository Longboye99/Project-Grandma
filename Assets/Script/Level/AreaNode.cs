using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;

public class AreaNode : MonoBehaviour
{
    public List<AreaPathInfo> pathInfos;

    public Dictionary<Direction, AreaEnum> directionDict = new Dictionary<Direction, AreaEnum>();
    public Dictionary<AreaEnum, AreaPathInfo> pathDict = new Dictionary<AreaEnum, AreaPathInfo>();
    
    public GameObject CameraPos;
    public AreaEnum area;

    private void Awake()
    {
        SetUpDirectionDict();
        SetUpPathDict();
    }
    private void SetUpDirectionDict()
    {
        foreach (var pathInfo in pathInfos)
        {
            Direction dir = pathInfo.direction;
            AreaEnum area = pathInfo.areaNode.area;

            directionDict.Add(dir, area);
        }
    }

    private void SetUpPathDict()
    {
        foreach(var pathInfo in pathInfos)
        {
            AreaEnum area = pathInfo.areaNode.area;
            AreaPathInfo path = pathInfo;

            pathDict.Add(area, path);
        }
    }
}

public enum Direction
{
    Default,
    Left,
    Right,
    Forward,
    Backward
}