using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerLobbyUiScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log ("playerLobbyUiScript: start");
		transform.SetParent (FindObjectOfType<Canvas> ().GetComponentInChildren<VerticalLayoutGroup> ().transform);	
	}
}
