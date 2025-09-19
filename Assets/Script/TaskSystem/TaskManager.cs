using System.Collections.Generic;
public class TaskManager : ITaskManager
{
    private readonly List<ITask> tasks = new();
    private readonly List<ITaskExecutor> executors = new();

    ITaskExecutor ITaskManager.CreateTaskExecutor()
    {
        return new TaskExecutor(this);
    }

    void ITaskManager.MarkAsIdle(ITaskExecutor executor)
    {
        if (tasks.Count > 0)
        {
            ITask task = tasks[0];
            executor.SetCurrentTask(task);
            tasks.Remove(task);
        }
        else
        {
            executors.Add(executor);
        }
    }

    void ITaskManager.PostTask(ITask task)
    {
        if (executors.Count > 0)
        {
            ITaskExecutor executor = executors[0];
            executor.SetCurrentTask(task);
            executors.Remove(executor);
        }
        else
        {
            tasks.Add(task);
        }
    }
}
