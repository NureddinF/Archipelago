﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//Class to control the hex menu
public class HexMenuController : MonoBehaviour
{
    //Parameters, 
    public GameObject hexMenu;
    private Text tileType;
    private Text tileStage; //TODO
    private Text tileIncome;
    private Image tileImage;
    private Text tileWorkerCount;
    private Text tileWarriorCount;
    private Image tileActionBox;
    

    //Parameter to store the current hex that the menu is displaying for.
    private Hex selectedHex;

    private void Start(){

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
    public Hex getSelectedHex()
    {
        return selectedHex;
    }

    //Method to set the currently selected hex
    public void setSelectedHex(Hex h)
    {
        //If the currently selected hex is same as clicked on, deselect the hex
        if (selectedHex != null && selectedHex.Equals(h))
        {
            deselectHex();
        }
        //Else select the hex
        else
        {
            selectedHex = h;
            //Set the hex menu parameteres to the hex's values 
            tileType.text = h.getTileType().ToString();
            //tileStage.text = h.getTileStage().ToString();
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
            tileWorkerCount.text = h.getNumOfWorkersOnHex().ToString();
            tileWarriorCount.text = h.getNumOfWarriorsOnHex().ToString();

            setTileActions();

            hexMenu.SetActive(true);
        }
    }

    //Method to deselect a hex
    public void deselectHex()
    {
        hideHexMenu();
        selectedHex = null;
    }
    //Method to hide the hex menu
    public void hideHexMenu() {
        hexMenu.SetActive(false);
    }

    //Method to move a worker to the selected hex
    public void moveWorkerToSelectedHex()
    {
        if (selectedHex != null)
        {
            gameObject.GetComponent<UnitController>().moveClosestAvailableWorker(selectedHex);
            tileWorkerCount.text = selectedHex.getNumOfWorkersOnHex().ToString();
        }
        else
            Debug.Log("No hex selected to move a worker unit to");
    }

    //Method to move a warrior to the selected hex
    public void moveWarriorToSelectedHex()
    {
        if (selectedHex != null)
        {
            gameObject.GetComponent<UnitController>().moveClosestAvailableWarrior(selectedHex);
            tileWarriorCount.text = selectedHex.getNumOfWarriorsOnHex().ToString();
        }
        else
            Debug.Log("No hex selected to move a warrior unit to");
    }

    //Refresh the ui values, useful for when the hex is still selected but changed values, e.g moved unit there, built something etc.
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

    //Set the tile actions part of the hex menu
    private void setTileActions()
    {
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
            float parentHeight = corners[2].y - corners[0].y;
            float parentWidth = corners[2].x - corners[0].x;

            //Float to store the percentage width for the child objects. Height if needed also
            float percentageWidth = 0.7f;
            //float percentageHeight = percentageWidth;

            //Floats to store the height and width of the child object, if equal 1:1 ratio
            float childWidth = percentageWidth * parentWidth;
            float childHeight = childWidth;

            // The distance between each menu items as well as the initial offset from top of action box
            float yOffset = 10f;
            //If the selected hex does not have a building
            if (selectedHex.getBuilding() == null)
            {
                //Get a list of possible building types that can be built on this hex
                List<Building> buildingOptions = gameObject.GetComponent<BuildingController>().getListOfBuildingByTileType(selectedHex.getTileType());

                //Count number of items iterated through to allow correct vertical displacement
                int count = 0;

                //For each building option
                foreach (Building b in buildingOptions)
                {
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
                    go.GetComponent<Button>().onClick.AddListener(() => { tileActionBuild(b); });
                    //Increment count
                    count++;
                }
            }
        }
    }

    //Method for the tileaction, when selecting a building
    void tileActionBuild(Building b)
    {
        selectedHex.setBuilding(b);
        refreshUIValues();
    }
}