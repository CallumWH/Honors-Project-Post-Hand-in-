using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedWeighting {

    PredatorCheck predatorCheckScript = new PredatorCheck();
    GameObject parentObject;
    GameObject sun;
    NodeKnowledgeBase knowledgeBase;

    private int predatorWeightFactor = -100;
    private int nightWeightFactor = -200;

    public int predatorCount;

    public float CalculateWeight(NeedsBase needInstance)
    {
        float currentValue = needInstance.GetValue();
        int priority = needInstance.priorityWeight;
        
        float internalWeight = (100.0f - currentValue) * (float) priority;

        int externalWeight = CalculateExternalInfluence(needInstance);
        float finalWeight = internalWeight + externalWeight;

        return finalWeight;
    }

    int CalculateExternalInfluence(NeedsBase needInstance)
    {
        int externalWeight = 0;
        
        //list of external weights, if the need has been told to consider it the calculation will be made here

        //Pedators
        if(needInstance.ConsiderPredators)
        {   //Number of predators nearby
            predatorCount = predatorCheckScript.CheckPredators(parentObject);

            if(needInstance.PredatorsPositive)
            {   
                //this will always calculate a negative value, so it's subtracted here to make it positive
                externalWeight -= predatorCount * predatorWeightFactor;
            }
            else
            {
                //calculates the negative value of nearby predators
                externalWeight += predatorCount * predatorWeightFactor;
            }
        }

        //Night Time

        if(needInstance.ConsiderNight)
        {
            bool sunUp;
            //WHERE'S THE SUN!? I NEED TO PRAISE IT
            if(!sun)
            {
                sun = GameObject.Find("Sun");
                Debug.Log("Found the Sun");
            }

            //Check what angle the sun (directional light) is
            Quaternion sunRotation = sun.GetComponent<Transform>().rotation;

            //is the sun up?
            if(sunRotation.eulerAngles.x < 90 && sunRotation.eulerAngles.x > 0)
            {
                sunUp = true;
            }
            else
            {
                sunUp = false;
            }

            //now do calculations based on perameters
            if(!sunUp)
            {
                 //is night a positive factor?
                 if(needInstance.NightPositive)
                 {
                    externalWeight -= nightWeightFactor;
                 }
                else       //night is negative
                {
                    externalWeight += nightWeightFactor;
                }
            }
            
        }
        //return final external weight value
        return externalWeight;
    }

    public int GetHighestWeight(List<NeedsBase> needList, GameObject parentObjectInput)
    {
        //set the parent
        parentObject = parentObjectInput;

        //if the knowledgebase isn't set, go get it

        if(!knowledgeBase)
        {
            knowledgeBase = parentObject.GetComponent<NodeKnowledgeBase>();
        }

        //calculate current weights
        for (int i = 0; i < needList.Count; i++)
        {
            needList[i].setWeight(CalculateWeight(needList[i]));
        }   

        //temp values for comparison of weights and sotring of index
        float currentHighestWeight = needList[0].GetWeight();
        int highestWeightIndex = 0;
        int secondHighestWeightIndex = 0;

        //once all weights are calculated, get the highest one
        for(int i = 0; i < needList.Count; i++)
        {
            //exempt any needs that do not have a known node
            //if this weight is higher than the current highest weight, replace it
            if (needList[i].GetWeight() > currentHighestWeight)
            {
                currentHighestWeight = needList[i].GetWeight();
                highestWeightIndex = i;
            }
        }

        //zero the current highest value (Index is already saved)
        currentHighestWeight = 0.0f;

        //now get the second highest weight
        for (int i = 0; i < needList.Count; i++)
            {
                //exempt any needs that do not have a known node
                if (needList[i].GetWeight() > currentHighestWeight && needList[i].GetWeight() < needList[highestWeightIndex].GetWeight())
                {
                    currentHighestWeight = needList[i].GetWeight();
                    secondHighestWeightIndex = i;
                }
            }

        //check if the two are the same

        if(highestWeightIndex == secondHighestWeightIndex)
        {
            //FIXITFIXITFIXITFIXITFIXITFIXITFIXITFIXITFIXITFIXITFIXITFIXITFIXIT
            float weight = needList[highestWeightIndex].GetWeight();
            weight += 0.1f;

            //reset the weight with a slight adjustment to break any deadlocks
            needList[highestWeightIndex].setWeight(weight);
        }

        //Debug.Log(needList[highestWeightIndex].needName + " is the highest weight");
        return highestWeightIndex;
    }
}
