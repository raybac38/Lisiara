using UnityEngine;

public interface ITask
{
  
    /// <summary>
    /// Execute the next step to the given settler
    /// </summary>
    /// <param name="settler"></param>
    public void NextStep(Settler settler);

    /// <summary>
    /// The task is aborted (canceled)
    /// Maybe it need to be reposte
    /// </summary>
    public void Abort();

    /// <summary>
    /// When the task finish or is aborted, where does the task need to be reposte ?
    /// </summary>
    /// <param name="taskManager"></param>
    public void SetTaskManagerCallback(ITaskManager taskManager);
    
}
