using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Unit {
	
	// Update is called once per frame
	void Update () {
            float step = unitSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destinationCoord, step);

            if (transform.position == destinationCoord)
            {
                GameObject.Find("Player").GetComponent<UnitController>().addWorkers(1, destinationHex);
                Destroy(gameObject);
            }        
    }
}
