using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGenerator : MonoBehaviour {

    private NodeManager nodeManager;

	// Use this for initialization
	void Start () {
        nodeManager = GameObject.Find("GameManager").GetComponent<NodeManager>();
	}
	
	// Update is called once per frame
	void Update () {

        for (int i = 0; i < nodeManager.nodeList.Count; i++)
        {

            if(nodeManager.nodeList[i].resourceAmount <= 0)
            {
                //Create a clone of a base resource node
                GameObject newNode = (GameObject)Instantiate(Resources.Load("Prefabs/BaseNodePrefab"));

                //copy over the perameters of the old node
                newNode.tag = nodeManager.nodeList[i].tag;
                newNode.GetComponent<Renderer>().material = nodeManager.nodeList[i].GetComponent<Renderer>().material;
                newNode.transform.position = nodeManager.nodeList[i].transform.position;
                newNode.name = nodeManager.nodeList[i].name;
                newNode.GetComponent<NodeLogic>().resourceAmount = Random.Range(50.0f, 120.0f);

                //generate a randomised co-ordinate to generate the new node
                Vector3 gameAreaMin = GameObject.Find("PlayArea").GetComponent<BoxCollider>().bounds.min;
                Vector3 gameAreaMax = GameObject.Find("PlayArea").GetComponent<BoxCollider>().bounds.max;

                Vector3 generatePosition;
                generatePosition.x = Random.Range(gameAreaMin.x, gameAreaMax.x);
                generatePosition.z = Random.Range(gameAreaMin.z, gameAreaMax.z);
                generatePosition.y = nodeManager.nodeList[i].transform.position.y;

                newNode.transform.position = generatePosition;

                //get rid of the old node
                NodeLogic toDelete = nodeManager.nodeList[i];
                nodeManager.nodeList.RemoveAt(i);

                toDelete.Delete();
            }
                }
	}
}
