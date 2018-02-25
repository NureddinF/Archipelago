using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour {
	// Unit that has been clicked
	private Unit selectedUnit;

	public Text incomeCount;
	public Sprite tileGrassOwned;	// Worth 2	
	public Sprite tileSandOwned;	// Worth 1
	public Sprite tileTreeOwned;	// Worth 3
	public Sprite tileRockOwned;	// Worth 4
	public Sprite unitDeselected;
	public Sprite unitSelected;

	public float zoomSpeed = 1f;
	public float panSpeed = 0.1f;
	public float i;
	public float rotx = 0f;
	public float roty = 0f;
	public float dir = -1;
	public float maxX = 120f;
	public float minX = -66f;
	public float maxY = 44f;
	public float minY = -50f;
	public Touch touchBegin = new Touch();
	public Vector3 original;
	public Vector2 finalPos;
	public Vector2 move;

	// Update is called once per frame
	void Update () {

		if (Input.touchCount == 2) {
			
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch (1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevTouchMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchMag = (touchZero.position - touchOne.position).magnitude;

			float magnitudeDiff = prevTouchMag - touchMag;

			Camera.main.orthographicSize += magnitudeDiff * zoomSpeed;;
			Camera.main.orthographicSize = Mathf.Max (Camera.main.orthographicSize, 5f);
			Camera.main.orthographicSize = Mathf.Min (Camera.main.orthographicSize, 30f);
		
		}

		if (Input.touchCount == 1) {
			original = Camera.main.transform.eulerAngles;
			rotx = original.x;
			roty = original.y;

			Touch touch = Input.GetTouch(0);
			Vector2 initTouch = touch.position;

			if (touch.phase == TouchPhase.Began) {
				touchBegin = Input.GetTouch (0);
			}
			if (touch.phase == TouchPhase.Moved) {

				float deltax = touchBegin.position.x - initTouch.x;
				float deltay = touchBegin.position.y - initTouch.y;

				rotx -= deltax * Time.deltaTime * panSpeed * dir;
				roty += deltay * Time.deltaTime * panSpeed * dir;

				if (rotx >= minX && rotx <= maxX) {
					
				}
					
				move = new Vector2 (Mathf.Clamp(rotx,-66,120),Mathf.Clamp(roty,-50,44));
				Camera.main.transform.Translate (move, Space.World);
		
			}

			if (touch.phase == TouchPhase.Ended) {
				touchBegin = new Touch ();
			}



//			Vector3 pos = Camera.main.ScreenToViewportPoint (finalPos);
//			Vector3 move = new Vector3 (pos.x * zoomSpeed, 0, pos.y * zoomSpeed);
//
//			Camera.main.transform.Translate (move, Space.World);

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
				if (collidedHitInfo.GetComponent<Hex> () != null) {
					Debug.Log ("Clicked On Hex");
					//clicked on a hex
					//List of direct cell neighbors
					/*List<GameObject> neighbors = collidedHitInfo.GetComponent<Hex> ().getNeighbors ();
					for (int i = 0; i < neighbors.Count; i++) {
						Debug.Log ("X: " + neighbors [i].GetComponent<Hex> ().x + " Y: " + neighbors [i].GetComponent<Hex> ().y);
					}*/


					// Check if we need to move unit to destination
					if (selectedUnit != null) {
						Vector3 v = collidedHitInfo.GetComponent<Hex> ().transform.position;
						Debug.Log ("Moving Unit to: x:" + v.x + ", y:" + v.y + ", z:" + v.z);
						selectedUnit.setDestination (collidedHitInfo.GetComponent<Hex> ().transform.position);
					} else {
						Debug.Log ("Hello World");
						//List of direct cell neighbors
						/*
               			 List<GameObject> neighbors = collidedHitInfo.GetComponent<Hex>().getNeighbors();
                		for (int i = 0; i < neighbors.Count; i++)
                		{
                		    Debug.Log("X: " + neighbors[i].GetComponent<Hex>().x + " Y: " +neighbors[i].GetComponent<Hex>().y);
               			 }
               			 */
						if (collidedHitInfo.tag == "Grass") {
							if(incomeCount.GetComponent<GenerateIncome> ().currentIncome >= 200) {
								collidedHitInfo.GetComponent<SpriteRenderer> ().sprite = tileGrassOwned;
								incomeCount.GetComponent<GenerateIncome> ().currentIncome -= 200;
								incomeCount.GetComponent<GenerateIncome> ().grassTerritory += 2;
							}

						} else if (collidedHitInfo.tag == "Sand") {
							if(incomeCount.GetComponent<GenerateIncome> ().currentIncome >= 100) {
								collidedHitInfo.GetComponent<SpriteRenderer> ().sprite = tileSandOwned;
								incomeCount.GetComponent<GenerateIncome> ().currentIncome -= 100;
								incomeCount.GetComponent<GenerateIncome> ().sandTerritory += 1;
							}

						} else if (collidedHitInfo.tag == "Tree") {
							if(incomeCount.GetComponent<GenerateIncome> ().currentIncome >= 300) {
								collidedHitInfo.GetComponent<SpriteRenderer> ().sprite = tileTreeOwned;
								incomeCount.GetComponent<GenerateIncome> ().currentIncome -= 300;
								incomeCount.GetComponent<GenerateIncome> ().treeTerritory += 3;
							}

						} else if (collidedHitInfo.tag == "Rock") {
							if(incomeCount.GetComponent<GenerateIncome> ().currentIncome >= 500) {
								collidedHitInfo.GetComponent<SpriteRenderer> ().sprite = tileRockOwned;
								incomeCount.GetComponent<GenerateIncome> ().currentIncome -= 500;
								incomeCount.GetComponent<GenerateIncome> ().rockTerritory += 4;
							}

						}
					}
				}
				else if (collidedHitInfo.GetComponent<Unit>() != null){
					//clicked on a unit
					Unit clickedUnit = collidedHitInfo.GetComponent<Unit>();
					if(clickedUnit == selectedUnit) deselectUnit();
					else selectUnit(clickedUnit);
				}


            }
        }
	}


	private void selectUnit(Unit unit){
		deselectUnit ();
		Debug.Log ("Selected unit: " + unit.name);
		this.selectedUnit = unit;
		//TODO: add UI things to indicate a unit was selected
		unit.GetComponent<SpriteRenderer>().sprite = unitSelected;
	}

	private void deselectUnit(){
		//TODO: Do UI things related to deslecting a unit
		if (selectedUnit != null) {
			SpriteRenderer temp = selectedUnit.GetComponent<SpriteRenderer> ();
			if (temp != null) {
				temp.sprite = unitDeselected;
			}
			this.selectedUnit = null;
		}
	}
}