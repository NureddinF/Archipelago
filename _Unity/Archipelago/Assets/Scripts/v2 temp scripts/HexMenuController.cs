using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HexMenuController : MonoBehaviour
{
    public GameObject hexMenu;
    private Text tileType;
    private Text tileStage; //TODO
    private Text tileIncome;
    private Image tileImage;
    private Text tileWorkerCount;
    private Text tileWarriorCount;

    private List<Image> actionOptions;

    private int warriorCount;
    private int workerCount;

    private Hex selectedHex;

    private void Start()
    {
        tileType = hexMenu.transform.Find("TileType").gameObject.GetComponent<Text>();
        tileStage = hexMenu.transform.Find("TileStage").gameObject.GetComponent<Text>();
        tileIncome = hexMenu.transform.Find("TileIncome").gameObject.GetComponent<Text>();
        tileImage = hexMenu.transform.Find("TileImage").gameObject.GetComponent<Image>();
        tileWorkerCount = hexMenu.transform.Find("TileWorkerCount").gameObject.GetComponent<Text>();
        tileWarriorCount = hexMenu.transform.Find("TileWarriorCount").gameObject.GetComponent<Text>();
        selectedHex = null;
        hideHexMenu();
    }

    public Hex getSelectedHex()
    {
        return selectedHex;
    }

    public void setSelectedHex(Hex h)
    {
        if (selectedHex != null && selectedHex.Equals(h))
        {
            deselectHex();
        }
        else
        {
            selectedHex = h;
            tileType.text = h.getTileType().ToString();
            //tileStage.text = h.getTileStage().ToString();
            float income = h.getTileIncome();

            if (income > 0)
            {
                tileIncome.color = Color.green; //TODO IMPROVE COLOUR CHOICES
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
            tileWorkerCount.text = gameObject.GetComponent<UnitController>().getWorkerCountByTileCoords(h.getX(), h.getY()).ToString();
            tileWarriorCount.text = gameObject.GetComponent<UnitController>().getWarriorCountByTileCoords(h.getX(), h.getY()).ToString();

            hexMenu.SetActive(true);
        }
    }

    public void deselectHex()
    {
        hideHexMenu();
        selectedHex = null;
    }

    public void hideHexMenu() {
        hexMenu.SetActive(false);
    }

    public void moveWorkerToSelectedHex()
    {
        if (selectedHex != null)
        {
            gameObject.GetComponent<UnitController>().moveClosestWorker(selectedHex);
            tileWorkerCount.text = gameObject.GetComponent<UnitController>().getWorkerCountByTileCoords(selectedHex.getX(), selectedHex.getY()).ToString();
        }
        else
            Debug.Log("No hex selected to move a worker unit to");
    }

    public void moveWarriorToSelectedHex()
    {
        if (selectedHex != null)
        {
            gameObject.GetComponent<UnitController>().moveClosestWarrior(selectedHex);
            tileWarriorCount.text = gameObject.GetComponent<UnitController>().getWarriorCountByTileCoords(selectedHex.getX(), selectedHex.getY()).ToString();
        }
        else
            Debug.Log("No hex selected to move a warrior unit to");
    }

    public void refreshUIValues()
    {
        if(selectedHex != null)
        {
            //Store current selected hex, deselect hex then reselect hex. This way on refresh if on same hex the menu wont hide itself
            Hex h = selectedHex;
            deselectHex();
            //"Reselect Hex" to update any changed values
            setSelectedHex(h);
        }
    }
}