using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public float unitSpeed = 2f;
    private Hex destinationHex;
    private Hex initialHex;
    private Vector3 currentCoord;
    private Vector3 destinationCoord;

	void Start () {
        transform.position = new Vector3(initialHex.transform.position.x, initialHex.transform.position.y, -5);
        destinationCoord = new Vector3(destinationHex.transform.position.x, destinationHex.transform.position.y, -5);
	}

    public void setInitialHex(Hex h) { initialHex = h; }
    public Hex getInitialHex() { return initialHex; }

    public void setDestinationHex(Hex h) { destinationHex = h; }
    public Hex getDestinationHex() { return destinationHex; }
}
