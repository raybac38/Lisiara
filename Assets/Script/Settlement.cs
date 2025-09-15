using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
   
    public GameObject SettlerPrefab;
    private List<Settler> settlers = new();
    private readonly ITaskManager taskManager = new TaskManager();


    public void AddNewSettler()
    {
        GameObject settlerObject = Instantiate(SettlerPrefab);
        Settler settler = settlerObject.GetComponent<Settler>();
        settler.executor = taskManager.CreateTaskExecutor();
        settler.settlement = this;
    }

    public void Start()
    {
        AddNewSettler();
        AddNewSettler();
        AddNewSettler();
    }
}
