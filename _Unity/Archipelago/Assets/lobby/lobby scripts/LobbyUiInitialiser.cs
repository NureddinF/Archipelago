using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUiInitialiser : MonoBehaviour {

	// Use this for initialization
	void Start () {
		FindObjectOfType<CustomLobbyManager> ().setLobbyUi (this.gameObject);
		gameObject.SetActive (false);
	}

}
