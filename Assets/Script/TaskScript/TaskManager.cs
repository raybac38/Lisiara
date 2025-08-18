using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Fonctionnement du Task Manager
/// Les tasks sont post�s sur une List
/// Les Settles se d�clare quand ils ont rien a faire
/// Si une task est post� 
/// </summary>

public class TaskManager
{
    private List<Task> taskList = new ();
    private List<Settler> idleSettles = new();

    public void PostNewTask(Task task)
    {
        taskList.Add(task);
    }

    public void MarkAsIdle(Settler settler)
    {
        idleSettles.Add(settler);
        if(taskList.Count == 0)
        {
            Task taks = new IdleTask(this, settler, 1f);
        }
    }
}
