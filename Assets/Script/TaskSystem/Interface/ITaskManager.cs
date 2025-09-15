using UnityEngine;

public interface ITaskManager
{
    /// <summary>
    /// Create a linked task executor
    /// </summary>
    public ITaskExecutor CreateTaskExecutor();

    /// <summary>
    /// Post a task to be executed
    /// </summary>
    /// <param name="task"></param>
    public void PostTask(ITask task);

    /// <summary>
    /// Notify that a task executor is waiting to recieve a new task
    /// </summary>
    /// <param name="executor"></param>
    public void MarkAsIdle(ITaskExecutor executor);
}
