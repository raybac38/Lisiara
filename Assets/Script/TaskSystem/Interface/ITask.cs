using UnityEngine;

public interface ITask
{

    public enum ExitStatus
    {
        FINISHED,   /// this task is finished
        RUNNING,    /// this task is running (can be recall)
        ERROR,      /// this task end in a error (a bug)
        ABORTED     /// this task end with abort (a condition for this is not satified)
    }
    /// <summary>
    /// Execute the next step of the task on the given settler
    /// </summary>
    /// <param name="settler"></param>
    public ExitStatus NextStep(Settler settler);

    /// <summary>
    /// Abort the current task
    /// </summary>
    public void Abort();
}
