using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour {
	// human player who is associated with this mouse manager
	public Player player;

	// Unit that has been clicked
	private Unit selectedUnit;

	// Building/unit creation
	public List<GameObject> unitPrefabs = new List<GameObject>();
	private int unitIndex = -1;
	private bool buildUnit = false;

	public float zoomSpeed = 1f;
	public float panSpeed = 0.1f;
	public float i;
	public float rotx = 0f;
	public float roty = 0f;
	public float dir;
	public float maxX;
	public float minX;
	public float maxY;
	public float minY;
	public float direction = -1;
	public Touch touchBegin = new Touch();
	public Vector2 initPos;
	public Vector3 original;
	public Vector2 finalPos;
	public Vector2 move;

	// Update is called once per frame
	void Update () {
		//Zoom
		if (Input.touchCount == 2) { //Checks for 2 touches on the screen
			
			Touch touchZero = Input.GetTouch(0); //stores the touches
			Touch touchOne = Input.GetTouch (1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; //gets the distance between touches
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevTouchMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; //magnitudes of the touches
			float touchMag = (touchZero.position - touchOne.position).magnitude;

			float magnitudeDiff = prevTouchMag - touchMag; 

			Camera.main.orthographicSize += magnitudeDiff * zoomSpeed;; //sets madnitude with zoomespeed to orthographicSize
			Camera.main.orthographicSize = Mathf.Max (Camera.main.orthographicSize, 5f); //sets the zoomout max size 
			Camera.main.orthographicSize = Mathf.Min (Camera.main.orthographicSize, 30f); //sets zoom-in max size
		
		}

		//panning
		if (Input.touchCount == 1) { //one touch on screen
			original = Camera.main.transform.eulerAngles;    
			rotx = original.x;
			roty = original.y;

			touchBegin = Input.GetTouch (0); //stores touch

			if (touchBegin.phase == TouchPhase.Began) { //checks touch phase
				initPos = touchBegin.position; //gets touch position
			}

			if (touchBegin.phase == TouchPhase.Moved) { //check phase if moved

				Vector2 touchDelta = touchBegin.deltaPosition; //stores delta postions of touch
				/*TODO: STOP CAMERA FROM PANNING OFF MAP*/

				Camera.main.transform.Translate (-touchDelta.x * panSpeed, -touchDelta.y * panSpeed, 0); //transforms the screen with pan sprred

			}
		
			if (touchBegin.phase == TouchPhase.Ended) { //touch phase ended
				touchBegin = new Touch (); //removes the touch 

			}
		}

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
                // Check what we clicked on
                if (collidedHitInfo.GetComponent<Hex>() != null)
                {
                    Hex hex = collidedHitInfo.GetComponent<Hex>();
                    //clicked on a hex


                    // Check if we need to move unit to destination
                    if (selectedUnit != null)
                    {
                        Vector3 v = hex.transform.position;
                        Debug.Log("Moving Unit to: x:" + v.x + ", y:" + v.y + ", z:" + v.z);
                        selectedUnit.setDestination(hex.transform.position);
                    }
                    else if (buildUnit)
                    {
                        //TODO: remove this condition
                        player.makeUnit(unitPrefabs[unitIndex]);
                    }
                    else
                    {
                        //bring up menu
                        //if(hex.hexOwner2 == player.playerId){
                        player.GetComponent<HexMenuController>().setSelectedHex(hex);
                        //}


                    }
                }
            } 
        }
	}


	private void selectUnit(Unit unit){
		deselectUnit ();
		this.selectedUnit = unit;
		//change sprite to indicate a unit was selected
		unit.selectUnit();
	}

	private void deselectUnit(){
		//change sprite to indicate a unit was deselected
		if (selectedUnit != null) {
			selectedUnit.deselectUnit ();
			this.selectedUnit = null;
		}
	}



	public void cycleNextUnit(){
		if (!buildUnit) {
			deselectUnit ();
			unitIndex = 0;
		} else {
			unitIndex++;
			if (unitIndex >= unitPrefabs.Count) {
				unitIndex = 0;
			}
		}
		buildUnit = true;
		Debug.Log ("Selecting to build unit: " + unitPrefabs [unitIndex].name);
	}
		
}
