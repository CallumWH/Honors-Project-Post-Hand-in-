using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour {

    [Range(0,20)]
    public float speed;

    //privates

    public float iterator = 0.0f;
	
	// Update is called once per frame
	void Update () {

        if(iterator > Mathf.PI*2)
        {
            iterator = 0.0f;
        }

        iterator += (speed * Time.deltaTime);

        float lightLevel = Mathf.Sin(iterator);


        gameObject.transform.Rotate(speed * Time.deltaTime, 0.0f, 0.0f);
	}
}
