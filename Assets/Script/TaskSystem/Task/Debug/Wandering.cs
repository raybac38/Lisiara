using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Wandering : ITask
{
    const float stepDuration = 0.5f;
    private Vector3Int currentPosition;
    private Vector3Int currentDestination;
    private List<Vector3Int> path = new();
    private MapData mapData = MapData.mapData;

    ITask.ExitStatus ITask.NextStep(Settler settler)
    {
        if(currentPosition == null || currentPosition != settler.gridPosition)
        {
            currentPosition = settler.gridPosition;
            
            bool pathExist = BasicPathfiding.Pathfiding.AStar(currentPosition, currentDestination, out path);
            while (!pathExist)  /// if the path doesn't exist, need to change destination
            {
                currentDestination = mapData.GetRandomCoordinate();
                pathExist = BasicPathfiding.Pathfiding.AStar(currentPosition, currentDestination, out path);
            }
        }

        Vector3Int nextPosition = path[0];
        bool mouvementSucces = settler.SetGridPosition(nextPosition);

        if (mouvementSucces)
        {
            currentDestination = nextPosition;
            path.RemoveAt(0);
        }

        if(path.Count == 0)
        {
            return ITask.ExitStatus.FINISHED;
        }else
        {
            return ITask.ExitStatus.RUNNING;
        }
    }

    void ITask.Abort()
    {
        throw new System.NotImplementedException();
    }
}
