using UnityEngine;

public class IdleTask : Task
{
    private readonly float duration;
    private float timer = 0f;
    
    public IdleTask(TaskManager manager, float duration) : base(manager)
    {
        this.duration = duration;
    }

    public override void NextStep(Settler settler)
    {
        timer += Time.deltaTime;
        if(timer >= duration)
        {
            isFinished = true;
        }
    }
}
