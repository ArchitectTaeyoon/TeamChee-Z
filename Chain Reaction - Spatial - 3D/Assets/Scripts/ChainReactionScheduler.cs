using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ExplosionTask
{
    public int i;
    public int j;
    public int k;

    public ExplosionTask(int _i, int _j, int _k)
    {
        i = _i;
        j = _j;
        k = _k;
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
            //if (Input.GetKeyDown(KeyCode.E))
           // {
                ExecuteExplosion(CurrentTask);
            //}
        }
    }


    //Schedule an explosion
    public void ScheduleExplosion(int i, int j, int k)
    {
        //First destroy the built space in the grid
        //GetComponent<GameBoardController>().GameBoardArray[i, j].GetComponent<BoardGrid>().VerticalNeighbourhood[k].GetComponent<GridUnit>().DestroyBuilt();
        //Schedule an explosion
        ExplosionSchedule.Add(new ExplosionTask(i, j, k));
        Debug.Log("EXPLOSION SCHEDULED: Grid(" + i.ToString() + ", " + j.ToString() + ", "+ k.ToString()+ ") set to explode as task: " + (ExplosionSchedule.Count-1).ToString());
        Debug.Log("Total number of Explosions: " + ExplosionSchedule.Count);
        GetComponent<GameBoardController>().GameBoardArray[i, j].GetComponent<BoardGrid>().VerticalNeighbourhood[k].GetComponent<GridUnit>().PrintNeighbourhood();
    }

    //Coroutine for the explosion
    public void ExecuteExplosion(int task)
    {
        Debug.Log("Entered");
        if(GetComponent<GameBoardController>().GameBoardArray[ExplosionSchedule[task].i, ExplosionSchedule[task].j].GetComponent<BoardGrid>().VerticalNeighbourhood[ExplosionSchedule[task].k].GetComponent<GridUnit>().CheckExplosionState())
        {
            Debug.Log("EXPLOSION STATUS: Task:" + task + " in process on Grid(" + ExplosionSchedule[task].i + ", " + ExplosionSchedule[task].j + ", "+ ExplosionSchedule[task].k+ ")");
            GetComponent<GameBoardController>().GameBoardArray[ExplosionSchedule[task].i, ExplosionSchedule[task].j].GetComponent<BoardGrid>().VerticalNeighbourhood[ExplosionSchedule[task].k].GetComponent<GridUnit>().Explode();
            taskComplete();
        }
        else
        {
            Debug.Log("EXPLOSION ABORTED: Task:" + task + " on Grid(" + ExplosionSchedule[task].i + ", " + ExplosionSchedule[task].j + ", " + ExplosionSchedule[task].k + ")");
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
