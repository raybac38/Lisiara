using System.Collections;
using UnityEngine;
public class TaskExecutor
{
    private Task currentTask = null;    // Task to be executed
    //private Task taskInSuspend = null;  // Save the suspended task here

    private readonly Settler owner;
    private readonly TaskManager taskManager;
    public TaskExecutor(Settler settler, TaskManager taskManager)
    {
        this.owner = settler;
        this.taskManager = taskManager;
        taskManager.MarkAsIdle(this);
    }

    public void AssigneTask(Task task)
    {
        Debug.Log("New task assignated");
        currentTask = task;
    }

    public void Update()
    {
        if(currentTask == null)
        {
            return;
        }
        if(currentTask.IsFinished)
        {
            currentTask = null;
            taskManager.MarkAsIdle(this);
        }
        else
        {
            currentTask.NextStep(owner);
        }
    }
}
