using UnityEngine;

public class Settler : MonoBehaviour, IEntity
{
    private TaskExecutor executor;
    public Settlement Settlement { get; set; }
    public Pathfiding Pathfiding { get; set; }
    public Map Map { get; set; }
    public Vector3Int GridPosition { get; set; }

    private void Awake()
    {
        Settlement = Settlement.settlement;
        Pathfiding = Settlement.Pathfiding;
        Map = Settlement.map;
        executor = new TaskExecutor(this, Settlement.TaskManager);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        executor.Update();
    }
}
