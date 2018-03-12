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
                tileIncome.text = "-" + income.ToString() + "/sec";
            }
            else
            {
                tileIncome.color = Color.grey;
                tileIncome.text = "0/sec";
            }

            tileImage.sprite = h.GetComponent<SpriteRenderer>().sprite;

            hexMenu.SetActive(true);
        }
    }

    public void deselectHex()
    {
        hideHexMenu();
        selectedHex = null;
        
    }

    public void hideHexMenu()
    {
        hexMenu.SetActive(false);
    }
}