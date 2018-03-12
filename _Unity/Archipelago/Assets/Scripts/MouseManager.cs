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

	//Right hand menu for building things
	public Menu menu;

	public float zoomSpeed = 0.07f;
	public float shiftSpeed = 10f;
	public float panSpeed = 0.07f;

	public Touch touchBegin = new Touch();
	public Vector2 initPos;
	public Vector3 original;
	public Vector2 finalPos;

	private Vector3 minBounds;
	private Vector2 maxBounds;
	private Camera main;
	private float halfHeight;
	private float halfWidth;
	public Vector3 centerPoint;
			
	void Start() {
		//https://www.youtube.com/watch?v=fQ2Dvj5-pfc
		main = Camera.main;

		halfHeight = main.orthographicSize;
		halfWidth = halfHeight * Screen.width / Screen.height;
	}

	// Update is called once per frame
	void Update () {

		//panning
		if (Input.touchCount == 1) { //one touch on screen

//			https://answers.unity.com/questions/517529/pan-camera-2d-by-touch.html , setting the boundary
//			https://answers.unity.com/questions/813224/move-camera-by-drag.html , creating a drag a zoom

			touchBegin = Input.GetTouch (0); //stores touch

			if (touchBegin.phase == TouchPhase.Began) { //checks touch phase

				initPos = touchBegin.position; //gets touch position
				original = main.ScreenToWorldPoint(initPos);
				original.z = main.transform.position.z;
			} else if (touchBegin.phase == TouchPhase.Moved) { //check phase if moved

				Debug.Log (touchBegin.phase);

				//Vector3 pos = main.ScreenToWorldPoint (touchBegin.position);
				//adds the difference of the fist position of the touch and the last touch after drag, increments that to the position
				Vector3 deltaTouch = (touchBegin.position  - initPos);
				Vector3 deltaCameraPos = main.ScreenToWorldPoint (touchBegin.position) - main.ScreenToWorldPoint (initPos); //TODO: this line???
				Vector3 newCameraPos = main.transform.position - deltaCameraPos;
						
				//Clamps the camera movement so it doens not go past the boundary
				float clammpedX = Mathf.Clamp (newCameraPos.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
				float clammpedY = Mathf.Clamp (newCameraPos.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);
				newCameraPos = new Vector3 (clammpedX, clammpedY, main.transform.position.z);
				initPos = touchBegin.position;
				main.transform.position = newCameraPos;

			} else { //touch phase ended
				touchBegin = new Touch (); //removes the touch 
				Debug.Log("Ended: "+touchBegin.phase);
			}
		}
		return;
		//Zoom
		if (Input.touchCount == 2) { //Checks for 2 touches on the screen

			Touch touchZero = Input.GetTouch(0); //stores the touches
			Touch touchOne = Input.GetTouch(1);

			if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began) {
				
				initPos = touchZero.position;
				finalPos = touchOne.position;

				centerPoint = new Vector3((main.ScreenToWorldPoint(initPos).x + main.ScreenToWorldPoint(finalPos).x) / 2f, (main.ScreenToWorldPoint(initPos).y + main.ScreenToWorldPoint(finalPos).y) / 2f,-10);
			
				float clammpedX = Mathf.Clamp (transform.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
				float clammpedY = Mathf.Clamp (transform.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);
				transform.position = new Vector3 (clammpedX, clammpedY, transform.position.z);

			}

			if(touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved){

				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; //gets the distance between touches
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
		
				float prevTouchMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; //magnitudes of the touches
				float touchMag = (touchZero.position - touchOne.position).magnitude;

				float magnitudeDiff = prevTouchMag - touchMag; 
	
				Camera.main.orthographicSize += magnitudeDiff * zoomSpeed * 2f; //sets magnitude with zoomspeed to orthographicSize

				Camera.main.orthographicSize = Mathf.Max (Camera.main.orthographicSize, 5f); //sets the zoomout max size 
				Camera.main.orthographicSize = Mathf.Min (Camera.main.orthographicSize, 12f); //sets zoom-in max size


				Camera.main.transform.position = Vector2.Lerp (Camera.main.transform.position, centerPoint * 2f, shiftSpeed * 2f);
				Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y, -10);

				halfHeight = main.orthographicSize;
				halfWidth = halfHeight * Screen.width / Screen.height;

				//Clamps the camera movement so it doens not go past the boundary
				float clammpedX = Mathf.Clamp (transform.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
				float clammpedY = Mathf.Clamp (transform.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);
				transform.position = new Vector3 (clammpedX, clammpedY, transform.position.z);
			
			}
			else {
				touchZero = new Touch ();
				touchOne = new Touch ();

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
			if (Input.GetMouseButtonDown(0)) {
				// Check what we clicked on
				if (collidedHitInfo.GetComponent<Hex> () != null) {
					Hex hex = collidedHitInfo.GetComponent<Hex> ();
					//clicked on a hex

					// Check if we need to move unit to destination
					if (selectedUnit != null) {
						Vector3 v = hex.transform.position;
						Debug.Log ("Moving Unit to: x:" + v.x + ", y:" + v.y + ", z:" + v.z);
						selectedUnit.setDestination (hex.transform.position);
					} else if (buildUnit){
						//TODO: remove this condition
						player.makeUnit (unitPrefabs[unitIndex]);
					} else {
						//bring up menu
						if(hex.hexOwner == player.playerId){
							menu.updateMenu(hex);	
						}


					}
				} else if (collidedHitInfo.GetComponent<Unit>() != null){
					//clicked on a unit
					Unit clickedUnit = collidedHitInfo.GetComponent<Unit>();
					if(clickedUnit == selectedUnit) deselectUnit();
					else selectUnit(clickedUnit);
					buildUnit = false;
					menu.updateMenu(null);
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
				

	public void setBounds (BoxCollider2D bounds){
		minBounds = bounds.bounds.min;
		maxBounds = bounds.bounds.max;
	}
}