using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HexMenuController : MonoBehaviour
{
    public GameObject hexMenu;

    private Text tileType;
    private string upgradeStatus;
    private string incomeRate;
    //TODO: private Sprite tileImage;
    //TODO: private string status;

    private List<Image> actionOptions;

    private int warriorCount;
    private int workerCount;

    private Hex selectedHex;

    private void Start()
    {
        tileType = hexMenu.transform.Find("TileType").gameObject.GetComponent<Text>();
        selectedHex = null;
        // hexMenu.enabled = false; used if hexMenu is image, however does not hide children
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
            tileType.text = h.tileType.ToString();
            //TODO FINISH THIS

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