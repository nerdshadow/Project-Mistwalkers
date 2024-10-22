using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public enum MapType
{
    Sand = 0,
    Stone = 1,
    City = 2,
    Void = 3
}
[Serializable]
public enum PathPointType
{
    Start = 0,
    End = 1,
    MidPath = 2,
}
[Serializable]
public struct PathPoint
{
    public PathPointType pathType;
    public MapType mapType;
    public CityData pathCity;
    public int rowIndex;
    public int difficulty;
    public UnityEngine.Random.State rStateOnCreating;
    public PathPoint(PathPointType _pathPointType, MapType _mapType, CityData _cityData, int _rowIndex, int _difficulty, UnityEngine.Random.State _rState)
    {
        pathType = _pathPointType;
        mapType = _mapType;
        pathCity = _cityData;
        rowIndex = _rowIndex;
        difficulty = _difficulty;
        rStateOnCreating = _rState;
    }
}
public static class PathGenerator
{
    public static int pathLength = 5;
    public static List<PathPoint> GeneratePath()
    {
        List<PathPoint> pathPoints = new List<PathPoint> ();
        for (int i = 0; i < pathLength; i++)
        {
            //Start point
            if (i == 0)
            {
                pathPoints.Add(new PathPoint(PathPointType.Start, MapType.City, GenerateCity(), i, 1, UnityEngine.Random.state));
                continue;
            }
            //End point
            if (i == pathLength - 1)
            {
                pathPoints.Add(new PathPoint(PathPointType.End, MapType.Void, GenerateCity(), i, 10, UnityEngine.Random.state));
                continue;
            }
            //Mid point
            pathPoints.Add(new PathPoint(PathPointType.MidPath, MapType.Sand, GenerateCity(), i, i + 1, UnityEngine.Random.state));
        }
        return pathPoints;
    }
    public static List<PathPoint> GeneratePath(int seed)
    {
        List<PathPoint> pathPoints = new List<PathPoint>();
        for (int i = 0; i < pathLength; i++)
        {
            //Start point
            if (i == 0)
            {
                pathPoints.Add(new PathPoint(PathPointType.Start, MapType.City, GenerateCity(), i, 1, UnityEngine.Random.state));
                continue;
            }
            //End point
            if (i == pathLength - 1)
            {
                pathPoints.Add(new PathPoint(PathPointType.End, MapType.Void, GenerateCity(), i, 10, UnityEngine.Random.state));
                continue;
            }
            //Mid point

            pathPoints.Add(new PathPoint(PathPointType.MidPath, MapType.Sand, GenerateCity(), i, i + 1, UnityEngine.Random.state));
        }
        return pathPoints;
    }

    static CityData GenerateCity()
    {
        string cName = "city" + UnityEngine.Random.Range(1, 64);
        RelatedFaction cFact = (RelatedFaction)UnityEngine.Random.Range(0, Enum.GetValues(typeof(RelatedFaction)).Length);
        List<ScriptableObject> cStock = new List<ScriptableObject>();
        return new CityData(cName, cFact, cStock);
    }
}
