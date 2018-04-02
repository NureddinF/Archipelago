using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//Class to control the hex menu
public class HexMenuController : NetworkBehaviour {
    //Parameters, 
    public GameObject hexMenu;
    private Text tileType;
    private Text tileStage; //TODO
    private Text tileIncome;
    private Image tileImage;
    private Text tileWorkerCount;
    private Text tileWarriorCount;
    private Image tileActionBox;

	public float workerCost = 25;
	public float warriorCost = 50;
    

    //Parameter to store the current hex that the menu is displaying for.
    private Hex selectedHex;

    private void Start(){

    }

	public void startWithAuthority(){
		if(!hasAuthority){
			//Only let local player update/access UI
			return;
		}

		hexMenu = GameObject.Find("Canvas").transform.Find("HexMenuBar").gameObject;

		//Use the hexMenu gameobject to find all it's individual UI elements.
		tileType = hexMenu.transform.Find("TileType").gameObject.GetComponent<Text>();
		tileStage = hexMenu.transform.Find("TileStage").gameObject.GetComponent<Text>();
		tileIncome = hexMenu.transform.Find("TileIncome").gameObject.GetComponent<Text>();
		tileImage = hexMenu.transform.Find("TileImage").gameObject.GetComponent<Image>();
		tileWorkerCount = hexMenu.transform.Find("TileWorkerCount").gameObject.GetComponent<Text>();
		tileWarriorCount = hexMenu.transform.Find("TileWarriorCount").gameObject.GetComponent<Text>();
		tileActionBox = hexMenu.transform.Find("TileActionBox").gameObject.GetComponent<Image>();
		selectedHex = null;
		hideHexMenu();
	}

    //Method to return the currently selected hex
    public Hex getSelectedHex() {
        return selectedHex;
    }

    //Method to set the currently selected hex
	//Updates local players UI
    public void setSelectedHex(Hex h) {
		//Get the id of the player
		Player.PlayerId pid = GetComponent<Player> ().playerId;
        //If the currently selected hex is same as clicked on, deselect the hex
        if (selectedHex != null && selectedHex.Equals(h)) {
            deselectHex();
        }
        //Else select the hex
        else {
            selectedHex = h;
            //Set the hex menu parameteres to the hex's values 
            tileType.text = h.getTileType().ToString();

            //Update the tilestage text
            if (h.getBuilding() != null && h.getBuilding().buildingId != Building.BuildingType.Trap)
                tileStage.text = h.getHexOwner().ToString() + ": " + h.getBuilding().buildingId.ToString().ToUpper();
            else
                tileStage.text = h.getHexOwner().ToString();


            float income = h.getTileIncome();

            //Set income and it's text colour based on it's amount
            if (income > 0)
            {
                tileIncome.color = new Color(0f, 0.8f, 0f); ; //TODO IMPROVE COLOUR CHOICES
                tileIncome.text = "+" + income.ToString() + "/sec";
            }
            else if (income < 0)
            {
                tileIncome.color = Color.red;
                tileIncome.text =  income.ToString() + "/sec";
            }
            else
            {
                tileIncome.color = Color.grey;
                tileIncome.text = "0/sec";
            }

            tileImage.sprite = h.GetComponent<SpriteRenderer>().sprite;
			tileWorkerCount.text = h.getNumOfWorkersOnHex(pid).ToString();
            tileWarriorCount.text = h.getNumOfWarriorsOnHex(pid).ToString();

            setTileActions();            

            hexMenu.SetActive(true);
        }
    }

    //Method to deselect a hex
    public void deselectHex() {
        hideHexMenu();
        selectedHex = null;
    }
    //Method to hide the hex menu
    public void hideHexMenu() {
        hexMenu.SetActive(false);
    }
		
    //Method to move a worker to the selected hex
    public void moveWorkerToSelectedHex() {
		//Get the id of the player
		Player.PlayerId pid = GetComponent<Player> ().playerId;
		Debug.Log ("HexMenuController: moveWorkerToSelectedHex: moving worker for " + pid);
        if (selectedHex != null) {
			gameObject.GetComponent<UnitController>().CmdMoveClosestAvailableWorker(selectedHex.gameObject);
            tileWorkerCount.text = selectedHex.getNumOfWorkersOnHex(pid).ToString();
        }
        else
            Debug.Log("No hex selected to move a worker unit to");
    }
		
    //Method to move a warrior to the selected hex
    public void moveWarriorToSelectedHex() {
		//Get the id of the player
		Player.PlayerId pid = GetComponent<Player> ().playerId;
		Debug.Log ("HexMenuController: moveWarriorToSelectedHex: moving warrior for " + pid);
        if (selectedHex != null) {
			gameObject.GetComponent<UnitController>().CmdMoveClosestAvailableWarrior(selectedHex.gameObject);
            tileWarriorCount.text = selectedHex.getNumOfWarriorsOnHex(pid).ToString();
        }
        else
            Debug.Log("No hex selected to move a warrior unit to");
    }
		
    //Refresh the ui values, useful for when the hex is still selected but changed values, e.g moved unit there, built something etc.
	[ClientRpc]
    public void RpcRefreshUIValues() {
		refreshUIValues ();
    }

	public void refreshUIValues(){
		if(selectedHex != null && hasAuthority)
		{
			//Store current selected hex, deselect hex then reselect hex. This way on refresh if on same hex the menu wont hide itself
			Hex h = selectedHex;
			deselectHex();
			//"Reselect Hex" to update any changed values
			setSelectedHex(h);
		}
	}

    //Set the tile actions part of the hex menu
    private void setTileActions() {
        //Get rid of any exisitng items for the action box
        foreach (Transform child in tileActionBox.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        //If hex is selected
        if (selectedHex)
        {
            //Array of vectors that store the corner world positions of the parent, tile action box
            Vector3[] corners = new Vector3[4];
            tileActionBox.GetComponent<RectTransform>().GetWorldCorners(corners);


            //Work out the action box's height and width, have to do it this way since rect transform has weird properties
            float parentWidth = corners[2].x - corners[0].x;

            //Float to store the percentage width for the child objects. Height if needed also
            float percentageWidth = 0.7f;
            //float percentageHeight = percentageWidth;

            //Floats to store the height and width of the child object, if equal 1:1 ratio
            float childWidth = percentageWidth * parentWidth;
            float childHeight = 0.65f * childWidth;

            // The distance between each menu items as well as the initial offset from top of action box
            float yOffset = 35f;
            //If the selected hex does not have a building and player owns the hex, then display build options
            if (selectedHex.getBuilding() == null &&selectedHex.getHexOwner().Equals(GetComponent<Player>().playerId)){
                //Get a list of possible building types that can be built on this hex
                List<Building> buildingOptions = gameObject.GetComponent<BuildingController>().getListOfBuildingByTileType(selectedHex.getTileType());

                //Count number of items iterated through to allow correct vertical displacement
                int count = 0;

                //For each building option
                foreach (Building b in buildingOptions)
                {
					Debug.Log (b);
                    //New gameobject
                    GameObject go = new GameObject();
                    //Set its parent
                    go.transform.parent = tileActionBox.transform;
                    //Set its name
                    go.name = b.name;
                    //Add appropriate components
                    go.AddComponent<RectTransform>();
                    go.AddComponent<Image>();
                    go.AddComponent<Button>();

                    //Set its rect transform properties
                    go.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                    go.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1f);
                    go.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1f);
                    
                    go.GetComponent<RectTransform>().sizeDelta = new Vector2(childWidth, childHeight);
                    go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -count * childHeight * go.GetComponent<RectTransform>().localScale.x - yOffset);
                    //Set its displayed sprite
                    go.GetComponent<Image>().sprite = b.getMenuIconSprite();
                    //Set its click function
					if (selectedHex.getTileType ().Equals (HexGrid.TileType.BASE)) {
						go.GetComponent<Button> ().onClick.AddListener (purchaseWorker);

					} else {

						go.GetComponent<Button> ().onClick.AddListener (() => {
							CmdTileActionBuild (selectedHex.gameObject , b.buildingId);
						});
					}
                    //Increment count
                    count++;
                }
            }
			else if(selectedHex.getBuilding() != null &&selectedHex.getHexOwner().Equals(GetComponent<Player>().playerId)){
				Building barracks = selectedHex.getBuilding ();
				if (barracks.buildingId.Equals (Building.BuildingType.Barracks)) {
					GameObject go = new GameObject ();
					//Set its parent
					go.transform.parent = tileActionBox.transform;
					//Set its name
					go.name = barracks.name;
					//Add appropriate components
					go.AddComponent<RectTransform> ();

					//Set its rect transform properties
					go.GetComponent<RectTransform> ().pivot = new Vector2 (0.5f, 1f);
					go.GetComponent<RectTransform> ().anchorMin = new Vector2 (0.5f, 1f);
					go.GetComponent<RectTransform> ().anchorMax = new Vector2 (0.5f, 1f);

					go.GetComponent<RectTransform> ().sizeDelta = new Vector2 (childWidth, childHeight);
					go.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0 * childHeight * go.GetComponent<RectTransform> ().localScale.x - yOffset);

					if (barracks.GetComponent<Barracks> ().getIsConstructed ()) {
						go.AddComponent<Image> ();
						go.AddComponent<Button> ();
						//Set its displayed sprite
						go.GetComponent<Image> ().sprite = barracks.GetComponent<Barracks> ().getpurchaseWarriorSprite ();

						go.GetComponent<Button> ().onClick.AddListener (purchasWarrior);
					}
				}


			}
        }
    }

	// Try to purchase a worker when the button is clicked
	private void purchaseWorker(){
		if (hasAuthority) {
			CmdPurchaseWorker (selectedHex.gameObject);
		}
	}

	// Check if player can afford worker and take approprate action
	[Command]
	private void CmdPurchaseWorker(GameObject barracksHex){
		if (workerCost > GetComponent<Player> ().getCurrentMoney ()) {
			//Can not afford worker
			Debug.Log ("CmdPurchaseWorker: Insufficent funds");
			RpcPurchaseWorkerFailed (barracksHex);
		} else {
			//Can afford worker
			GetComponent<Player> ().removeMoney (workerCost);
			GetComponent<UnitController> ().CmdAddWorkers (1, barracksHex);
			RpcPurchaseWorkerSuccess (barracksHex);
		}
	}


	// Blink worker count red on failure
	[ClientRpc]
	private void RpcPurchaseWorkerFailed(GameObject barracksHex){
		if(hasAuthority && selectedHex.gameObject == barracksHex){
			StartCoroutine (purchaseWorkerResult(Color.red));
		}
	}

	// Blink worker count green on sucess
	[ClientRpc]
	private void RpcPurchaseWorkerSuccess(GameObject barracksHex){
		if (hasAuthority && selectedHex.gameObject == barracksHex) {
			tileWorkerCount.text = selectedHex.getNumOfWorkersOnHex(GetComponent<Player>().playerId).ToString();
			Color green = new Color (0, 0.75f, 0);
			StartCoroutine (purchaseWorkerResult (green));
		}
	}

    //Blink the worker count with a color
	IEnumerator purchaseWorkerResult(Color flashingColor){
		tileWorkerCount.color = flashingColor;
		yield return new WaitForSeconds(0.5f);
		tileWorkerCount.color = Color.black;
	}

	// Try to purchase a warrior when the button is clicked
	private void purchasWarrior(){
		if (hasAuthority) {
			CmdPurchaseWarrior (selectedHex.gameObject);
		}
	}

	// Check if player can afford warrior and take approprate action
	[Command]
	private void CmdPurchaseWarrior(GameObject barracksHex){
		if (warriorCost > GetComponent<Player> ().getCurrentMoney ()) {
			// can not afford a warrior
			Debug.Log ("CmdPurchaseWorker: Insufficent funds");
			RpcPurchaseWarriorFailed (barracksHex);
		} else {
			// can affoed a warrior
			GetComponent<Player> ().removeMoney (warriorCost);
			GetComponent<UnitController> ().CmdAddWarriors (1, barracksHex);
			RpcPurchaseWarriorSuccess (barracksHex);
		}
	}

	// Blink warrior count red on failure
	[ClientRpc]
	private void RpcPurchaseWarriorFailed(GameObject barracksHex){
		if(hasAuthority && selectedHex.gameObject == barracksHex){
			StartCoroutine (purchaseWarriorResult(Color.red));
		}
	}

	// Blink warrior count green on sucess
	[ClientRpc]
	private void RpcPurchaseWarriorSuccess(GameObject barracksHex){
		if (hasAuthority && selectedHex.gameObject == barracksHex) {
			tileWarriorCount.text = selectedHex.getNumOfWarriorsOnHex(GetComponent<Player>().playerId).ToString();
			Color green = new Color (0, 0.75f, 0);
			StartCoroutine (purchaseWarriorResult (green));
		}
	}

	//Blink the warrior count with a color
	IEnumerator purchaseWarriorResult(Color flashingColor){
		tileWarriorCount.color = flashingColor;
		yield return new WaitForSeconds(0.5f);
		tileWarriorCount.color = Color.black;
	}

	//OnClick listener for buildings
	void tileActionBuild(Building.BuildingType buildingId){
		CmdTileActionBuild (selectedHex.gameObject, buildingId);
	}
		
	// Command to build a building
	[Command]
	void CmdTileActionBuild(GameObject tile, Building.BuildingType buildingId){
		tile.GetComponent<Hex> ().CmdSetBuilding (buildingId);
		//Refresh hex menu's values to display these changes
		RpcRefreshUIValues ();
	}
}