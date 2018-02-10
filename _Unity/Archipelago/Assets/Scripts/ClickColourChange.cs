using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickColourChange : MonoBehaviour {

    void start()
    {
        this.GetComponent<SpriteRenderer>().color = Color.grey;
    }

	void OnMouseEnter(){
        this.GetComponent<SpriteRenderer>().color = Color.yellow;
    }
    void OnMouseExit(){
        this.GetComponent<SpriteRenderer>().color = Color.grey;

    }

}
