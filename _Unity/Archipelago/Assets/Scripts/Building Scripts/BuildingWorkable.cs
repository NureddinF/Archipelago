using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingWorkable : Building {
    private int numberAssignedWorkers;
    public int maxNumberOfAssignedWorkers = 5;
    public float increasedIncomePerWorker = 0.5f;

    void Start() {
		if (!isServer) {
			return;
		}
        setCurrentTileIncome(getTileIncomeAfterBuild());
    }

    public int getNuberAssignedWorkers() { return numberAssignedWorkers; }

    public float getIncreasedIncomePerWorker() { return increasedIncomePerWorker; }

    public void allocateWorker()
    {
        if (numberAssignedWorkers < maxNumberOfAssignedWorkers)
        {
            numberAssignedWorkers += 1;
            setCurrentTileIncome(getCurrentTileIncome() + increasedIncomePerWorker);
        }
        else
            Debug.Log("Can't assign worker since currently there are the maximum amount possible assigned to this building");
    }

    public void deallocateWorker()
    {
        if (numberAssignedWorkers > 0)
        {
            numberAssignedWorkers -= 1;
            setCurrentTileIncome(getCurrentTileIncome() - increasedIncomePerWorker);
        }
        else
            Debug.Log("Can't deallocate worker since currently there are none assigned to this building");
    }

}
