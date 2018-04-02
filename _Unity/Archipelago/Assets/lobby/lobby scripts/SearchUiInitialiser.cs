using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchUiInitialiser : MonoBehaviour {

	// Use this for initialization
	void Start () {
		FindObjectOfType<CustomLobbyManager> ().setSearchUi (this.gameObject);
		GetComponentInChildren<Button> ().onClick.AddListener (FindObjectOfType<CustomLobbyManager> ().attemptConnection);
		FindObjectOfType<Canvas>().transform
			.Find ("ExitButton").GetComponent<Button>()
			.onClick.AddListener(FindObjectOfType<CustomLobbyManager> ().returnToAndroidmenu);
	}
}
