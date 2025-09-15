using UnityEngine;

public interface ITaskExecutor
{
    /// <summary>
    /// Set the current task to the task executor
    /// </summary>
    /// <param name="task"></param>
    public void SetCurrentTask(ITask task);

    /// <summary>
    /// Update the current task
    /// </summary>
    public void NextStep(Settler settler);

    /// <summary>
    /// Abort the current task
    /// </summary>
    public void Abort();

}
