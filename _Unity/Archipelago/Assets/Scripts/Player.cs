using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	public static bool isEnabled = true;
    // Identify who this player is
    public enum PlayerId { P1, P2, NEUTRAL };
    public PlayerId playerId;

    // Player parameters
    public float rateMultiplier;
    public int baseIncome;
    public int timeFrame; // how long between discrete burst of money
	public float moneyToWin = 100;

    //The parent gameobject for main UI bar
    public GameObject stateMenu;

    // UI
    private Text incomeText;
    private Text rateText;
    private Text tilesOwnedText;
    private Text numOfWorkersOwned;
    private Text numOfWarriorsOwned;

    // State variables
    private float startTime;
    private float totalTileIncome;
    private float currentMoney;
    private int totalTilesOwned;
	
	private WinPanel winPanel; //calls on winPanel object

    // Reference to the map so Player can access tiles
    public HexGrid map;

    // Use this for initialization
    void Start()
    {
        //Connect all UI script elements to their gameobjects
        incomeText = stateMenu.transform.Find("Resource Icon/Total Amount Text").gameObject.GetComponent<Text>();
        rateText = stateMenu.transform.Find("Resource Icon/Increase Rate Text").gameObject.GetComponent<Text>();
        tilesOwnedText = stateMenu.transform.Find("Territory Icon/Total Territory Text").gameObject.GetComponent<Text>();
        numOfWorkersOwned = stateMenu.transform.Find("Worker Button/Worker Count Text").gameObject.GetComponent<Text>();
        numOfWarriorsOwned = stateMenu.transform.Find("Warrior Button/Warrior Count Text").gameObject.GetComponent<Text>();

        //Initialize Variables
        currentMoney = 0;
        incomeText.text = "Hello";
        rateText.text = "+ 0/sec";
        tilesOwnedText.text = "0";
        startTime = Time.time;
        numOfWarriorsOwned.text = gameObject.GetComponent<UnitController>().initialNumOfWarriors.ToString();
        numOfWorkersOwned.text = gameObject.GetComponent<UnitController>().initialNumOfWorkers.ToString();
    }
	
	void Awake(){
		winPanel = WinPanel.instance ();
	}

    // Update is called once per frame
    void Update()
    {
		//to pause script
		if (isEnabled) {
			return;
		}
        // Use discrete chunks of income
        generateIncomeDiscrete();
		
		if (currentMoney >= moneyToWin ) { //when money threshold reached, and no winner is false
			rateText.text = "WIN";
			incomeText.text = "";
		
			Time.timeScale = 0f; //pauses game
			Hex.isEnabled = true;    //Calls on these scripts to disable them, so the game cannot be played when a winner is found
			MouseManager.isEnabled = true;
			isEnabled = true;
		
			winPanel.back("You Win!"); //calls on back in winPanel class
		}
	
        numOfWarriorsOwned.text = "" + gameObject.GetComponent<UnitController>().getTotalNumberOfWarriors();
        numOfWorkersOwned.text = "" + gameObject.GetComponent<UnitController>().getTotalNumberOfWorkers();
        tilesOwnedText.text = "" + totalTilesOwned;
    }

    //Getters/Setters
    public float getCurrentMoney() { return currentMoney; }

    public void removeMoney(float amount) { currentMoney -= amount; }

    // Generates income in discrete chunks rather than each frame
    public void generateIncomeDiscrete()
    {
        float increaseAmt = (baseIncome + totalTileIncome) * rateMultiplier;
        if (Time.time > timeFrame + startTime)
        {
            // timeframe has passed
            currentMoney += increaseAmt;
            startTime = Time.time;
        }
        incomeText.text = "" + currentMoney;
        rateText.text = "+ " + increaseAmt + "/" + timeFrame + "secs";
    }

	//Capture a Tile
    public void captureTile(CapturableTile tile)
    {
        totalTileIncome += tile.getHex().getTileIncome();
        totalTilesOwned += 1;
        this.GetComponent<HexMenuController>().refreshUIValues();
    }

	//Lose a Tile
    public void removeTile(CapturableTile tile)
    {
        totalTileIncome -= tile.getHex().getTileIncome();
        totalTilesOwned -= 1;
    }
}
