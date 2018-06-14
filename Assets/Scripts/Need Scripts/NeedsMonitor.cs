using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedsMonitor : MonoBehaviour {

    //PUBLICS
    public List<NeedsBase> needList;

    //PRIVATES

    //NEEDS LIST FUNCTIONALITY
    private int focusNeed;
    private int listSize;

    private bool fillingNeed;
    private bool fillNeedStateInit;
    private enum currentState { none, fillNeed, searchNeed };
    private string needTag;
    private GameObject needNodeObject;
    private NeedWeighting weightCalculator = new NeedWeighting();
    private Collider collider;

    //GameManager
    private NodeManager nodeManager;

    //KnowledgeBase
    private NodeKnowledgeBase knowledgeBase;

    //OTHER FUNCTIONALITY
    private Vector3 oldPosition;
    private int oldPredatorCount;

    currentState state;
    

	// Use this for initialization
	void Start ()
    {
        //set the game manager
        nodeManager = GameObject.Find("GameManager").GetComponent<NodeManager>();
        if(!nodeManager)
        {
            Debug.LogError("cannot locate node manager");
        }

        //set the knowledge base
        knowledgeBase = gameObject.GetComponent<NodeKnowledgeBase>();
        if(!knowledgeBase)
        {
            Debug.LogError("no knowledge base attached to " + gameObject.name);
        }

        listSize = needList.Count;

        for (int i = 0; i < needList.Count; i++)
        {
            if(!needList[i].Init())
            {
                Debug.Log("Error on element : " + i + " On object : " + gameObject.name);
            }
            else
            {
                Debug.Log(i + " : No errors");
            }
        }


        //add needs to the UI script (if there is one attached)

        if (gameObject.GetComponentInChildren<NeedsUIScript>())
        {
            gameObject.GetComponentInChildren<NeedsUIScript>().addToList();
        }

        if (gameObject.GetComponentInChildren<WeightUIScript>())
        {
            gameObject.GetComponentInChildren<WeightUIScript>().addToList();
        }

        oldPosition = gameObject.transform.position;

        //default state
        state = currentState.none;
        
    }

    // Update is called once per frame
    void Update()
    {
        //get a copy of the knowledge list and clear out any nulls (threaded list element removal is pain)
        if (!knowledgeBase)
        {
            Debug.LogError("Knowledge base for " + gameObject.name + " not found");
        }

        List<NodeLogic> knowledgeList = new List<NodeLogic>(knowledgeBase.GetList());
        List<NodeLogic> toRemove = new List<NodeLogic>();

        for (int i = 0; i < knowledgeList.Count; i++)
        {
            if (knowledgeList[i] == null)
            {
                toRemove.Add(knowledgeList[i]);
            }
        }
        for (int i = 0; i < toRemove.Count; i++)
        {
            knowledgeList.Remove(toRemove[i]);
        }

        switch (state)
        {
            case currentState.none:

                focusNeed = weightCalculator.GetHighestWeight(needList, gameObject);

                //Debug.Log(gameObject.name + needList[focusNeed].needName + " : Has been selected with a weight value of : " + needList[focusNeed].GetWeight());
                state = currentState.fillNeed;

                break;


            case currentState.fillNeed:

                //do init
                if (!fillNeedStateInit)
                {
                    //get the tag for what fills the triggered need
                    needTag = needList[focusNeed].GetFulfillmentTag();

                    //find a node for the triggered need
                    for (int i = 0; i < knowledgeList.Count; i++)
                    {
                        if (needTag == knowledgeList[i].tag)
                        {
                            needNodeObject = knowledgeList[i].gameObject;
                        }
                    }

                    //INITIALIZE COMPLETE
                    fillNeedStateInit = true;
                }

                //if the node isn't known, switch to searching behaviour           
                if (needNodeObject == null)
                {
                    state = currentState.searchNeed;
                    fillNeedStateInit = false;
                    break;
                }
                else
                {

                    //move to that node
                    gameObject.GetComponent<PlayerMovement>().MoveTo(needNodeObject.transform.position);
                    //Debug.Log("Moving to " + needNodeObject.name);

                    //if we are inside the node, restore it
                    if (fillingNeed)
                    {
                        //start restoring the hunger
                        needList[focusNeed].setDrain(-10.0f);
                        needNodeObject.GetComponent<NodeLogic>().SetDrain(-10.0f);

                        //once full, switch state back to normal
                        if (needList[focusNeed].GetValue() > 100.0f)
                        {
                            state = currentState.none;
                            //needList[focusNeed].resetDrain();
                            needNodeObject.GetComponent<NodeLogic>().SetDrain(0.0f);


                            //CLEANUP

                            FillNeedCleanup();
                        }
                    }
                    else
                    {
                        needList[focusNeed].resetDrain();
                        needNodeObject.GetComponent<NodeLogic>().SetDrain(0.0f);
                    }
                }

                break;

            case currentState.searchNeed:
                {
                    //Go for a walk
                    gameObject.GetComponent<PlayerMovement>().Wander(); //Start Wandering

                    //Poll other needs and break wandering if one takes over
                    for(int i = 0; i < needList.Count; i++)
                    {
                        if(needList[i].GetWeight() > needList[focusNeed].GetWeight())
                        {
                            //return to the base state
                            state = currentState.none;
                            break;
                        }
                    }

                    //if it finds a node, return to the fulfilment state
                    for (int i = 0; i < knowledgeList.Count; i++)
                    {
                        if (knowledgeList[i].tag == needList[focusNeed].GetFulfillmentTag())
                        {
                            //go back to fulfilment state
                            state = currentState.fillNeed;
                            break;
                        }
                    }
                    

                    break;
                }
        }

        //state independant code----------------------------------------------------------

        //Check if the colider has been deleted while we are in it
        if (!collider && fillingNeed == true)
        {
            fillingNeed = false;
            needList[focusNeed].resetDrain();
        }

        for (int i = 0; i < listSize; i++)
        {
            needList[i].Update();

            //continuously outputs weights (not recommended, but ok for debug)
            weightCalculator.GetHighestWeight(needList, gameObject);
        }

        if(fillingNeed == false)
        {
            needList[focusNeed].resetDrain();
        }

        //Danger Check

        //check if there are more predators nearby than before
        if (oldPredatorCount < weightCalculator.predatorCount)
        {
            //re-evaluate course of action
            FillNeedCleanup();
        }

        //Stop if a need hits 0

        for(int i = 0; i < listSize; i++)
        {
            if(needList[i].GetValue() < 0)
            {
                Debug.Log(needList[i].needName + "has bottomed out with time lasted : " + Time.time);
                Debug.Break();
            }
        }


        //remember the current predator count
        oldPredatorCount = weightCalculator.predatorCount;

       
    }

    void OnTriggerEnter(Collider collider)
    {
        
        if(collider.gameObject.tag == needList[focusNeed].GetFulfillmentTag())
        {
            fillingNeed = true;
            Debug.Log(gameObject.name + "has entered " + collider.gameObject.name);
            this.collider = collider;
        }
        else
        {
            fillingNeed = false;
        }
    }

    void OnTriggerExit(Collider collider)
    {      
            fillingNeed = false;
            Debug.Log(gameObject.name + "has exited " + collider.gameObject.name);
    }

    void FillNeedCleanup()
    {
        //CLEANUP

        //Clear the tag of the need
        needTag = string.Empty;

        //Clear the target node that was used
        needNodeObject = null;

        //reset the init
        fillNeedStateInit = false;

        //reset the focus node's drains
        needList[focusNeed].resetDrain();

        //head back to the weight evaluation
        state = currentState.none;

    }

    public int GetListSize() { return listSize; }
}
