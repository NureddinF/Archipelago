﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	public Vector3 destination;
	public int speed = 1;
	public bool shouldMove = false;
	
	// Update is called once per frame
	void Update () {
		if (shouldMove) {
			moveTowards(destination);
		}
	}

	private void moveTowards(Vector3 dest){
		// Get vector from current positon to destination
		Vector3 max_distance = dest - transform.position;
		// Calculate velocity vecotor
		Vector3 velocity = max_distance.normalized * speed;
		// Calculate distance vector this to travel this frame
		Vector3 actual_distance = velocity * Time.deltaTime;
		// Clamp distance vector so we don't overshoot target
		actual_distance = Vector3.ClampMagnitude (actual_distance, max_distance.magnitude);

		// Update position
		this.transform.Translate(actual_distance);
		if(this.transform.position.Equals(destination)){
			reachedWaypoint();
		}
	}

	public void setDestination(Vector3 dest){
		dest.z = this.transform.position.z;
		this.destination = dest;
		shouldMove = true;
	}
	
	private void reachedWaypoint(){
		// TODO: pathfinding
		shouldMove = false;
	}
}