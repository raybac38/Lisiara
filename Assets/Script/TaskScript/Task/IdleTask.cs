using UnityEngine;

public class IdleTask : Task
{
    private readonly float timer;
    public IdleTask(TaskManager manager, Settler owner, float timer) : base(manager, owner)
    {
        this.timer = timer;
    }

    public override float NextStep()
    {
        isFinished = true;
        return timer;
    }
}
