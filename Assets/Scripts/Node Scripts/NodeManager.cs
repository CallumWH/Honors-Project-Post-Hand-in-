using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour {

    public List<NodeLogic> nodeList;

    public List<GameObject> nodePrefabList;

    //privates
    BoxCollider playArea;
    static bool updateLocked = false;
    float time = 0;
    //float delayTime = 0;

	// Use this for initialization
	void Start () {

        //get the play area
        playArea = GameObject.Find("PlayArea").GetComponent<BoxCollider>();

	}
	
	// Update is called once per frame
	public void Update() {
    }

    //Call this to add a node to the list
    public void addToList(NodeLogic node)  
    {
        nodeList.Add(node);
    }


    public bool CheckIfExists(string tag)                       //Checks if the need tag exists within the list of nodes
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (tag == nodeList[i].tag)
            {
                return true;
            }
        }
        return false;
    }
}
