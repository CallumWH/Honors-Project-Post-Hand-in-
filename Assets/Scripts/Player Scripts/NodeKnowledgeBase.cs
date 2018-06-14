using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeKnowledgeBase : MonoBehaviour {

    //Publics
    
    public float perceptionRange = 20.0f;

    //Privates
    private NodeManager nodeManager;
    private bool locked = false;
    public List<NodeLogic> knownNodes = new List<NodeLogic>();


	// Use this for initialization
	void Start () {
        nodeManager = GameObject.Find("GameManager").GetComponent<NodeManager>();

	}

    // Update is called once per frame
    void Update()
    {
        //get distances to each node
        for (int i = 0; i < nodeManager.nodeList.Count; i++)
        {
            //distance calculation
            float distance = Vector3.Distance(nodeManager.nodeList[i].gameObject.transform.position, gameObject.transform.position);

            //check if the object is close to the node
            if (distance < perceptionRange)
            {
                if(!knownNodes.Contains(nodeManager.nodeList[i]))
                {
                    //Add it to the known objects list
                    knownNodes.Add(nodeManager.nodeList[i]);
                    Debug.Log(gameObject.name + " is adding " + nodeManager.nodeList[i]);
                }           
            }

        }

        List<NodeLogic> toRemove = new List<NodeLogic>();
        //check for nulls
        for (int i = 0; i < knownNodes.Count; i++)
        {
            if (knownNodes[i] == null)
            {
                //get rid of it
                toRemove.Add(knownNodes[i]);               
            }
        }
        for(int i = 0; i < toRemove.Count; i++)
        {
            knownNodes.Remove(toRemove[i]);
        }
    }

    public bool CheckIfExists(string tag)                       //Checks if the need tag exists within the list of nodes
    {
        List<NodeLogic> toRemove = new List<NodeLogic>();
        //check for nulls
        for (int i = 0; i < knownNodes.Count; i++)
        {
            if (knownNodes[i] == null)
            {
                //get rid of it
                toRemove.Add(knownNodes[i]);
            }
        }
        for (int i = 0; i < toRemove.Count; i++)
        {
            knownNodes.Remove(toRemove[i]);
        }


        //check the list for the tag
        for (int i = 0; i < knownNodes.Count; i++)
        {
            if (tag == knownNodes[i].tag)
            {
                //Debug.Log(gameObject.name + " " + tag + " exists");
                return true;
            }
        }
        //Debug.Log(gameObject.name + " " + tag + " does not exists");
        return false;
    }

    public List<NodeLogic> GetList()
    {
        return knownNodes;
    }
}
