using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour {

	// Update is called once per frame
	void Update () {

        // mouse location, provides coordinates relative to screen pixels
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;

        //Screen pos is relative to camera location in unity coordinates
        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

        //Information regarding the object the ray collides with
        //Returns true or false but also provides information of object collider coliided with
        RaycastHit2D hitInfo = Physics2D.Raycast(screenPos, Vector2.zero);

        //If ray collides with an object
        if (hitInfo)
        {   
            //Return the gameobject that the ray has collided with
            GameObject collidedHitInfo = hitInfo.collider.transform.gameObject;

            //If left mouse button pressed, only calls once on initial press(e.g not constantly calling on hold)
            if (Input.GetMouseButtonDown(0))
            {
                //List of direct cell neighbors
                List<GameObject> neighbors = collidedHitInfo.GetComponent<Hex>().getNeighbors();
                for (int i = 0; i < neighbors.Count; i++)
                {
                    Debug.Log("X: " + neighbors[i].GetComponent<Hex>().x + " Y: " +neighbors[i].GetComponent<Hex>().y);
                }
            }
        }
	}
}
