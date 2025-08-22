using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    public static Settlement settlement;
    [SerializeField]
    private GameObject settlerPrefab;
    private readonly List<Settler> settlers = new();
    public TaskManager TaskManager { get; } = new();
    public Pathfiding Pathfiding { get; }
    [SerializeField]
    public Map map;

    private void Awake()
    {
        if(settlement != null)
        {
            Debug.LogError("Instance of Settlement already existe");
        }
        else
        {
            settlement = this;
        }
        
    }

    private void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            SpawnSettler(new Vector3Int(50, 0, 50 + i));
        }
    }

    public void SpawnSettler(Vector3Int gridPosition)
    {
        int yPosition = map.GetSurfaceY(new (gridPosition.x, gridPosition.z));
        GameObject gameObject = Instantiate(settlerPrefab);
        gameObject.transform.position = new Vector3(gridPosition.x, yPosition, gridPosition.z);
        Settler settler = gameObject.GetComponent<Settler>();
        if (settler == null) {
            Debug.LogError("Settler prefab don't have Settler Script");
        }
        settlers.Add(settler);
        settler.GridPosition = new Vector3Int(gridPosition.x, yPosition, gridPosition.z);
    }

    public void RemoveSettler(Settler t)
    {

    }
}
