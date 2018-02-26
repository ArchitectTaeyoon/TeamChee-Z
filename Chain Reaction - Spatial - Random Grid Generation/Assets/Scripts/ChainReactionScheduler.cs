using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ExplosionTask
{
    public int i;
    public int j;

    public ExplosionTask(int _i, int _j)
    {
        i = _i;
        j = _j;
    }
}

public class ChainReactionScheduler : MonoBehaviour {

    //Variable
    public int TotalTasks = 0;
    public int CurrentTask = 0;

    //List that hold the tasks
    public List<ExplosionTask> ExplosionSchedule;

	// Use this for initialization
	void Start () {
        ExplosionSchedule = new List<ExplosionTask>();
	}
	
	// Update is called once per frame
	void Update () {
        if (ExplosionSchedule.Count != 0)
        {
            ExecuteExplosion(CurrentTask);
        }
    }

    //Schedule an explosion
    public void ScheduleExplosion(int i, int j)
    {
        //First destroy the built space in the grid
        GetComponent<GameBoardController>().GameBoardArray[i, j].GetComponent<GridUnit>().DestroyBuilt();
        //Schedule an explosion
        ExplosionSchedule.Add(new ExplosionTask(i, j));
        Debug.Log("EXPLOSION SCHEDULED: Grid(" + i.ToString() + "," + j.ToString() + ") set to explode as task: " + (ExplosionSchedule.Count-1).ToString());
        GetComponent<GameBoardController>().GameBoardArray[i, j].GetComponent<GridUnit>().PrintNeighbourhood();
    }

    //Coroutine for the explosion
    public void ExecuteExplosion(int task)
    {
        if(GetComponent<GameBoardController>().GameBoardArray[ExplosionSchedule[task].i, ExplosionSchedule[task].j].GetComponent<GridUnit>().CheckExplosionState())
        {
            Debug.Log("EXPLOSION STATUS: Task:" + task + " in process on Grid(" + ExplosionSchedule[task].i + "," + ExplosionSchedule[task].j + ")");
            GetComponent<GameBoardController>().GameBoardArray[ExplosionSchedule[task].i, ExplosionSchedule[task].j].GetComponent<GridUnit>().Explode();
            taskComplete();
        }
        else
        {
            Debug.Log("EXPLOSION ABORTED: Task:" + task + " on Grid(" + ExplosionSchedule[task].i + "," + ExplosionSchedule[task].j + ")");
            taskComplete();
        }

    }

    //Clear Schedule
    public void ClearSchedule()
    {
        ExplosionSchedule.Clear();
        CurrentTask = 0;
    }

    //Get info from the exploding unit if the task is complete
    public void taskComplete()
    {
        CurrentTask++;
    }
}
