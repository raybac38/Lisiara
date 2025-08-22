using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Fonctionnement du Task Manager
/// Les tasks sont postés sur une List
/// Les Settles se déclare quand ils ont rien a faire
/// Si une task est posté 
/// </summary>

public class TaskManager
{
    private List<Task> taskList = new ();
    private List<TaskExecutor> idleTaskExecutor = new();

    public void PostNewTask(Task task)
    {
        taskList.Add(task);
    }

    public void TaskAborted(Task task)
    {
        throw new NotImplementedException("TaskManager TaskAborted is not implemented yet");
    }

    public void RemoveTask(Task task)
    {
        throw new NotImplementedException("TaskManager RemoveTask is not implemented yet");
    }

    public void MarkAsIdle(TaskExecutor taskExecutor)
    {
        idleTaskExecutor.Add(taskExecutor);
        if(taskList.Count == 0)
        {
            Task task = new WanderingTask(this);
            taskExecutor.AssigneTask(task);
            Debug.Log("No task to assigne. Putting to Idle");
        }
        else
        {
            Task task = taskList[0];
            taskList.RemoveAt(0);
            taskExecutor.AssigneTask(task);
            Debug.Log("Task assignated");
        }
    }
}
