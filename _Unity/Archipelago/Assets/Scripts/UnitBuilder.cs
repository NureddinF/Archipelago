using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBuilder : MonoBehaviour {

    //Bool to store whether in unit placing stage or not
    private bool buttonSelected = false;
    private bool over = false;
    public GameObject unit;

    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    private void OnMouseOver()
    {
        over = true;
    }

    private void OnMouseExit()
    {
        over = false;
    }

    void OnMouseDown()
    {
        if (over)
        {
            buttonSelected = true;
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void OnMouseUp()
    {
        if(!over && buttonSelected)
        {
            GameObject thisUnit;
            thisUnit = (GameObject)Instantiate(unit);
            Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            thisUnit.transform.position = new Vector3(temp.x, temp.y, -5);
        }
        buttonSelected = false;
        gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

}
