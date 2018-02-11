using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour {

	// Update is called once per frame
	void Update () {

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;

        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);


        RaycastHit2D hitInfo = Physics2D.Raycast(screenPos, Vector2.zero);

        if (hitInfo)
        {
            GameObject collidedHitInfo = hitInfo.collider.transform.gameObject;
            
            if (Input.GetMouseButtonDown(0)){
				
                List <GameObject> neighbors = collidedHitInfo.GetComponent<Hex>().getNeighbors();
                Debug.Log(neighbors);
            }

        }
	}
}
