using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeedsBase
{

    //Define drain rates for the needs
    public string needName; //Name of this need
    public float drainRate; //This is Per Second, passes this to current drainrate durring init
    public string fulfillmentTag; //What fufils this need?

    //Bools for External Needs
    public bool ConsiderPredators;
    public bool ConsiderNight;

    //Bools for inverting external values
    public bool PredatorsPositive;
    public bool NightPositive;
    
    [Range(1, 5)]
    public int priorityWeight; //needs inherant priority, higher values are greater importance

    //PRIVATES
    private float currentValue; //Value for the need, this needs a better variable name
    private float currentWeight;
    private float currentDrainRate; //This is the drain rate that's actually used in code


    // Use this for initialization
    public bool Init()
    {
        //randomise the starting value
        currentValue = Random.Range(90.0f, 100.0f);

        //Mosty error catching, but a couple of setup functions too
        //check if a name was inputed
        if (needName.Length.Equals(0))
        {
            Debug.LogError("name not defined for need");
            return false;
        }

        //Set the initial weighting value, also catches if it wasn't initialized
        if (priorityWeight <= 0)
        {
            Debug.LogError("forgot to initialize weighting for " + needName);
            return false;
        }

        //Check if a node tag is defined
        if (fulfillmentTag.Length == 0)
        {
            Debug.LogError("No tag defined for need fufulment for " + needName);
            return false;
        }

        //Checks if the drain rate has a value that makes sense
        if (drainRate <= 0)
        {
            Debug.LogError("Non-sensicle value or uninitilized drain rate for " + needName);
            return false;
        }
        else
        {
            resetDrain();
        }

        return true;
    }

    // Update is called once per frame
    public void Update()
    {
        //get the delta time
        currentValue -= (currentDrainRate * Time.deltaTime);
    }

    public float GetValue() { return currentValue; }

    public string GetFulfillmentTag() { return fulfillmentTag; }

    public void setWeight(float newWeight) { currentWeight = newWeight; }

    public float GetWeight() { return currentWeight; }

    public void setDrain (float newDrainRate) { currentDrainRate = newDrainRate; }

    public void resetDrain() { currentDrainRate = drainRate; }
}
