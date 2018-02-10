using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverChange : MonoBehaviour {

    private Color startColor;

    void OnMouseEnter(){
        startColor = GetComponent<SpriteRenderer>().color;
        this.GetComponent<SpriteRenderer>().color = Color.yellow;
    }
    void OnMouseExit(){
        this.GetComponent<SpriteRenderer>().color = startColor;

    }

}
