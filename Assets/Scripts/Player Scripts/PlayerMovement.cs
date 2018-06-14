using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 5.0f;
    public float maxTimeToWander = 5.0f;

    private GameObject playArea;

    //wandering vars
    private float wanderTime;
    private bool wanderGenerateCoord = true;
    private Vector3 generatePosition;

    // Use this for initialization
    void Start () {
        playArea = GameObject.Find("PlayArea");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MoveTo(Vector3 targetPosition)
    {
        //calculate the relative position of the players target local to the player
        Vector3 relativePosition = targetPosition - gameObject.transform.position;

        //normalise it
        relativePosition = relativePosition.normalized;

        //get rid of Y value
        relativePosition.y = 0.0f;

        //Move to the poition
        gameObject.transform.Translate((relativePosition * speed) * Time.deltaTime);
    }

    public void Wander()
    {
        //regenerate co-ordinate after a set time
        if (wanderGenerateCoord)
        {
            //generate a random co-ordinate on the map
            Vector3 gameAreaMin = GameObject.Find("PlayArea").GetComponent<BoxCollider>().bounds.min;
            Vector3 gameAreaMax = GameObject.Find("PlayArea").GetComponent<BoxCollider>().bounds.max;

            generatePosition.x = Random.Range(gameAreaMin.x, gameAreaMax.x);
            generatePosition.z = Random.Range(gameAreaMin.z, gameAreaMax.z);
            generatePosition.y = 0.0f;

            wanderTime = 0.0f;
            wanderGenerateCoord = false;

            Debug.Log("Co-ordinate generated");

        }

        //Move to the poition
        MoveTo(generatePosition);

        //increase timer
        wanderTime += Time.deltaTime;

        if(wanderTime > 5.0f)
        {
            wanderGenerateCoord = true;
        }
    }

}
