using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour {

	public static bool isEnabled = false;
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
	public float shiftSpeed = 0.1f;
	public float panSpeed = 0.07f;
	public float cameraMovementTolerance = 0.5f;

	public Vector2 clickPos;
	private bool canPerformSelection = false;

	private Vector2 minBounds;
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
		//to pause script
		if (isEnabled) {
			return;
		}

		// Handle clicking
		if (Input.touchCount == 0) {
			//For Debugging on PC
			if(Input.GetMouseButtonDown(0)){
				canPerformSelection = true;
				clickPos = Input.mousePosition;
			}
			// Check input status
			if(canPerformSelection){
				// Check if anything was clicked
				canPerformSelection = false;
				doSelection ();
			}
		}
		//panning
		else if (Input.touchCount == 1) { //one touch on screen

//			https://answers.unity.com/questions/517529/pan-camera-2d-by-touch.html , setting the boundary
//			https://answers.unity.com/questions/813224/move-camera-by-drag.html , creating a drag a zoom

			Touch singleTouch = Input.GetTouch (0); //stores touch
			if (singleTouch.phase == TouchPhase.Began) { //checks touch phase
				//gets touch position
				clickPos = singleTouch.position; 
				canPerformSelection = true;
			} else if (singleTouch.phase == TouchPhase.Moved) { //check phase if moved
				panCamera(singleTouch.position, clickPos);
				clickPos = singleTouch.position;
			} else if(singleTouch.phase == TouchPhase.Ended){ //touch phase ended
				Debug.Log("Ended: "+singleTouch.phase);
			}


		}

		//Zoom
		else if (Input.touchCount == 2) { //Checks for 2 touches on the screen

			//Get the touches
			Touch touchZero = Input.GetTouch(0); //stores the touches
			Touch touchOne = Input.GetTouch(1);

			// Check if a touch ended
			bool touchZeroEnded = touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled;
			bool touchOneEnded = touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled;

			//checks touch phase
			if (touchZeroEnded && !touchOneEnded) {
				// Need to make sure panning works smoothly after leaving this state
				clickPos = touchOne.position; 
			} else if (touchOneEnded && !touchZeroEnded){
				// Need to make sure panning works smoothly after leaving this state
				clickPos = touchZero.position; 
			} else if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began) {
				// New touch, find center point
				centerPoint = touchZero.position + (touchOne.position - touchZero.position)/2f;

			} else if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved) { //check phase if moved
				// At least one finger moved, zoom camera towards center of touches
				// Save starting world point for focused zoom on center of pinch
				Vector3 oldCenterWorldPoint = main.ScreenToWorldPoint(centerPoint);

				// Zoom camera
				float zoomDir = pinchZoomCamera(touchZero, touchOne);
				// Pan camera so it doesn't just zoom on center of screen
				Vector3 newTouchCenterPoint = touchZero.position + (touchOne.position - touchZero.position)/2f;
				Vector3 diffBetweenCameraAndCenter = main.ScreenToWorldPoint(newTouchCenterPoint) - oldCenterWorldPoint;
				main.transform.position = clampNewCameraPos (main.transform.position - diffBetweenCameraAndCenter);

				// Update state
				centerPoint = newTouchCenterPoint;
			}
		}
	}

	// Check if player clicked on something
	private void doSelection(){
		// mouse location, provides coordinates relative to screen pixels
		Vector3 mousePos = new Vector3 (clickPos.x, clickPos.y, 0);

		//Screen pos is relative to camera location in unity coordinates
		Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

		//Information regarding the object the ray collides with
		//Returns true or false but also provides information of object collider coliided with
		RaycastHit2D hitInfo = Physics2D.Raycast(screenPos, Vector2.zero);

		//If ray collides with an object
		if (hitInfo) {
			
			//Return the gameobject that the ray has collided with
			GameObject collidedHitInfo = hitInfo.collider.transform.gameObject;
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

	// Pan camera from old position to new position
	private Vector3 panCamera(Vector3 newTouchPos, Vector3 oldTouchPos){
		//adds the difference of the fist position of the touch and the last touch after drag, increments that to the position
		Vector3 deltaCameraPos = main.ScreenToWorldPoint (newTouchPos) - main.ScreenToWorldPoint (oldTouchPos);
		Vector3 newCameraPos = main.transform.position - deltaCameraPos; // negative to make it feel like dragging map, not camera

		//Clamps the camera movement so it doens not go past the boundary
		main.transform.position = clampNewCameraPos(newCameraPos);

		//Don't perform selection if camera movement happens
		if(deltaCameraPos.magnitude > cameraMovementTolerance){
			canPerformSelection = false;
		}
		return newCameraPos;
	}

	// Zoom in/out based on two touches
	private float pinchZoomCamera(Touch touchZero, Touch touchOne){

		//gets the distance between touches
		Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; 
		Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

		//magnitudes of the touches
		float prevTouchMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; 
		float touchMag = (touchZero.position - touchOne.position).magnitude;
		float magnitudeDiff = prevTouchMag - touchMag; 

		//Zoom camera
		Camera.main.orthographicSize += magnitudeDiff * zoomSpeed * 2f; //sets magnitude with zoomspeed to orthographicSize
		float cameraMaxZoom = 15f;//Mathf.Min(Mathf.Abs(maxBounds.y), Mathf.Abs(maxBounds.x))/2;
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 5f, cameraMaxZoom); //sets the zoomout max size 

		//Update screen size vars
		halfHeight = main.orthographicSize;
		halfWidth = halfHeight * Screen.width / Screen.height;

		//Don't perform selection if camera movement happens
		if(magnitudeDiff > cameraMovementTolerance){
			canPerformSelection = false;
		}

		//Return weather this was a zoom out or in
		return Mathf.Sign (magnitudeDiff);
	}

	// Ensure camera stays within bounding box (so player doesnt scroll/zoom out to infinity)
	private Vector3 clampNewCameraPos(Vector3 newCameraPos){
		float clammpedX = Mathf.Clamp (newCameraPos.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
		float clammpedY = Mathf.Clamp (newCameraPos.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);
		return new Vector3 (clammpedX, clammpedY, main.transform.position.z);
	}
}