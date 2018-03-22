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

    // UI
    public Text incomeText;
    public Text rateText;
    public Text numOfTilesOwned;
    public Text numOfWorkersOwned;
    public Text numOfWarriorsOwned;
    public Text tilesOwnedText;
    public Image redBarTemp;

    // State variables
    private float startTime;
    private float totalTileIncome;
    private float currentMoney;
    private int totalTilesOwned;
	
	private WinPanel winPanel; //calls on winPanel object

    // Reference to the map so Player can access tiles
    public HexGrid map;

    // List of owned hexes
    private List<Hex> hexesOwned;

    // Use this for initialization
    void Start()
    {
        currentMoney = 0;
        incomeText.text = "Hello";
        rateText.text = "+ 0/sec";
        tilesOwnedText.text = "0";
        startTime = Time.time;
        hexesOwned = new List<Hex>();
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
        redBarTemp.transform.localScale = new Vector3((float)totalTilesOwned/((float)totalTilesOwned+1), 1, 1);
    }

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

    public void captureTile(CapturableTile tile)
    {
        totalTileIncome += tile.getHex().getTileIncome();
        totalTilesOwned += 1;
        this.GetComponent<HexMenuController>().refreshUIValues();
    }

    public void removeTile(CapturableTile tile)
    {
        totalTileIncome -= tile.getHex().getTileIncome();
    }

    public void makeUnit(GameObject unitObject)
    {
       //complete method
    }
}
