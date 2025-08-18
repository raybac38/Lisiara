using UnityEngine;

public abstract class Task
{
    protected readonly Settler owner;
    protected readonly TaskManager manager;
    protected bool isFinished = false;
    /// <summary>
    /// Assigne the task to the Settler
    /// </summary>
    /// <param name="owner"></param>
    public Task(TaskManager manager, Settler owner)
    {
        this.owner = owner;
        this.manager = manager;
    }
    /// <summary>
    /// Is the current task finished
    /// </summary>
    public bool IsFinished { get { return isFinished; } }
    /// <summary>
    /// Execute the next step for the current task
    /// </summary>
    /// <returns>Time in second about the duration of this step</returns>
    public abstract float NextStep();


}
