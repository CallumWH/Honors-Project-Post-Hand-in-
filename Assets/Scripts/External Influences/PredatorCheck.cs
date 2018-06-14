using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorCheck
{

    private List<GameObject> predators;
    public int tollerance = 10;     //tweakable


	void ListPredators()                //lists all of the active predators in the game
    {
        predators = new List<GameObject>(GameObject.FindGameObjectsWithTag("Predator")); //get all the predators that exist when this is called
	}
	
	//Calculate how many predators are nearby
	public int CheckPredators (GameObject parentObject)
    {
        Vector3 playerVector = parentObject.transform.position;
        Vector3 targetPredator;
        int predatorsNearby = 0;


        ListPredators();
        
        if (predators.Count != 0)
        {
            for (int i = 0; i < predators.Count; i++)
            {
                targetPredator = predators[i].transform.position;
                float distance = Vector3.Distance(playerVector, targetPredator);

                if(distance < tollerance)
                {
                    predatorsNearby++;
                }
            }
        }

        return predatorsNearby;
	}
}
