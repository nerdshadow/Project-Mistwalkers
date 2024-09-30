using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public enum MapType
{
    Sand = 0,
    Stone = 1,
    City = 2
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
    public int rowIndex;
    public int difficulty;
    public PathPoint(PathPointType _pathPointType, MapType _mapType, int _rowIndex, int _difficulty)
    {
        pathType = _pathPointType;
        mapType = _mapType;
        rowIndex = _rowIndex;
        difficulty = _difficulty;
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
                pathPoints.Add(new PathPoint(PathPointType.Start, MapType.City, i, 1));
                continue;
            }
            //End point
            if (i == pathLength - 1)
            {
                pathPoints.Add(new PathPoint(PathPointType.End, MapType.City, i, 10));
                continue;
            }
            //Mid point
            pathPoints.Add(new PathPoint(PathPointType.MidPath, MapType.Sand, i, i + 1));
        }
        return pathPoints;
    }
}
