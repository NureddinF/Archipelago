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
    private Image tileActionBox;
    private List<Sprite> buildOptions;

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
        tileActionBox = hexMenu.transform.Find("TileActionBox").gameObject.GetComponent<Image>();
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

            setTileActions();

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

    private void setTileActions()
    {

        foreach (Transform child in tileActionBox.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (selectedHex)
        {
            if (!selectedHex.getBuilding())
            {
                List<Building> buildingOptions = gameObject.GetComponent<BuildingController>().getListOfBuildingByTileType(selectedHex.getTileType());

                Vector3[] corners = new Vector3[4];
                tileActionBox.GetComponent<RectTransform>().GetWorldCorners(corners);

                float parentHeight = corners[2].y - corners[0].y;
                float parentWidth = corners[2].x - corners[0].x;

                float percentageWidth = 0.7f;
                float percentageHeight = percentageWidth;

                float childWidth = percentageWidth * parentWidth;
                float childHeight = childWidth;

                float yOffset = 10f; //The distance between each menu items as well as the initial offset from top of action box

                //Count number of items iterated through to allow correct vertical displacement
                int count = 0;
                
                foreach (Building b in buildingOptions)
                {
                    GameObject go = new GameObject();
                    go.transform.parent = tileActionBox.transform;
                    go.name = b.name;
                    go.AddComponent<RectTransform>();
                    go.AddComponent<Image>();
                    go.AddComponent<Button>();
                    go.AddComponent<AspectRatioFitter>();
                    go.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
                    go.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                    go.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1f);
                    go.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1f);
                    go.GetComponent<RectTransform>().sizeDelta = new Vector2(childWidth, childHeight);
                    go.GetComponent<Image>().sprite = b.getMenuIconSprite();
                    go.GetComponent<Button>().onClick.AddListener(delegate { tileActionBuild(b); });
                    go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -count*childHeight*go.GetComponent<RectTransform>().localScale.x -yOffset);
                    count++;
                }
            }
            else {
                //INSERT CODE TO SHOW MENU DISPLAY FOR BUILDING ON THAT TILE
            }
        }
    }

    void tileActionBuild(Building b)
    {
        selectedHex.GetComponent<CapturableTile>().beginConstruction(b);
    }
}