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
		
}
