using System.Collections.Generic;
using UnityEngine;

public class WanderingTask : Task
{
    private Vector3Int startingPosition;
    private List<Vector3Int> path = new();
    private Vector3Int endingPosition;
    private Settler owner;
    private bool isInitialized = false;
    public WanderingTask(TaskManager manager) : base(manager)
    {

    }

    public override void NextStep(Settler settler)
    {
        if (isInitialized)
        {
            Vector3Int nextPosition = path[0];
            path.RemoveAt(0);
            settler.transform.position = nextPosition;
            settler.GridPosition = nextPosition;

            if(path.Count <= 0)
            {
                isFinished = true;
            }
        }
        else
        {
            // Initialisation
            owner = settler;

            Pathfiding pathfiding = Pathfiding.pathfiding;
            Map map = Settlement.settlement.map;
            startingPosition = settler.GridPosition;

            Vector2Int endingCoordinate = new Vector2Int((int)Random.Range(-50f, 50f),(int)Random.Range(-50f, 50f)) + new Vector2Int(startingPosition.x, startingPosition.z);
            
            int yPosition = map.GetSurfaceY(endingCoordinate);

            bool isAccessible = pathfiding.AStar(startingPosition, new(endingCoordinate.x, yPosition, endingCoordinate.y), out path);
            if(!isAccessible)
            {
                isFinished = true;
            }
            isInitialized = true;
        }
    }
}
