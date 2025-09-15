using UnityEngine;

public class TaskExecutor : ITaskExecutor
{
    private ITaskManager taskManager; 
    private ITask currentTask;
 
    public TaskExecutor(ITaskManager taskManager)
    {
        this.taskManager = taskManager;
        taskManager.MarkAsIdle(this);
    }

    void ITaskExecutor.Abort()
    {
        if(currentTask == null)
        {
            Debug.LogError("Current task is null, cannot be aborted");
            return;
        }
        currentTask.Abort();
        currentTask = null;
    }

    void ITaskExecutor.NextStep(Settler settler)
    {
        if(currentTask == null)
        {
            Debug.LogError("Current task is null, cannot execute next step");
            return;
        }
        currentTask.NextStep(settler);
    }

    void ITaskExecutor.SetCurrentTask(ITask task)
    {
        if(currentTask != null)
        {
            Debug.LogError("current task not null, cannot set a new task");
            return;
        }
        currentTask = task;
    }
}
