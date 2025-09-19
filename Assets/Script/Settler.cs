using System.Collections;
using UnityEngine;

public class Settler : MonoBehaviour
{
    public Settlement settlement;

    public ITaskExecutor executor;

    public Vector3Int gridPosition; /// PASSER ICI POUR SET UNE BONNE COORDONNEE

    private bool canMove = true;
    
    public bool SetGridPosition(Vector3Int gridPosition)
    {
        if (!canMove) return false;
        this.gridPosition = gridPosition;
        transform.position = gridPosition;
        return true;
    }

    private void Update()
    {
        executor.NextStep(this);
    }
}
