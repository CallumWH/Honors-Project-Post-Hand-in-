using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLogic : MonoBehaviour {

    //PUBLIC
    public float resourceAmount = 10.0f; //100.0f;

    //PRIVATE

    private NodeManager nodeManager;

	// Use this for initialization
	void Start () {
        nodeManager = GameObject.Find("GameManager").GetComponent<NodeManager>();
        nodeManager.addToList(this);
	}

	// Update is called once per frame
	void Update () {
		
	}

    public void Delete()
    {
        Destroy(gameObject);
    }

    public void SetDrain(float drainRate)
    {
        resourceAmount += drainRate * Time.deltaTime;
    }

}
