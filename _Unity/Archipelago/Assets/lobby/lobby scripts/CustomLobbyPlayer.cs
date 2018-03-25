using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CustomLobbyPlayer : NetworkLobbyPlayer {

	public GameObject PlayerUiPrefab; //prefab of lobby UI for player info
	private GameObject playerUI; //reference to spawned version of player UI
	[SyncVar (hook = "initLobbyPlayerPid")] private Player.PlayerId pid;
	[SyncVar (hook = "initLobbyPlayerUsername")] private string username;
	[SyncVar (hook = "initLobbyPlayerIp")] private string ipAddress;

	public static Player.PlayerId nextPid = Player.PlayerId.P1;

	//////////////////////////////////////Monobehaviour Methods//////////////////////////////////////////////

	// Use this for initialization
	void Start () {
		Debug.Log("CustomLobbyPlayer: Start");
		if(isLocalPlayer){
			Debug.Log ("CustomLobbyPlayer: Start: isLocalPlayer");
			CmdInitLobbyPlayer (InitialGameState.username, InitialGameState.ClientIpAddr);
			//initLobbyPlayer ("User " + Random.Range(0,100));
			Debug.Log ("CustomLobbyPlayer: Start: isLocalPlayer: "
				+ "username: " + username + ", ipAddress: " + ipAddress
				+ "player: " + pid.ToString());
		} else {
			Debug.Log ("CustomLobbyPlayer: Start: NOT isLocalPlayer: "
				+ "username: " + username + ", ipAddress: " + ipAddress
				+ "player: " + pid.ToString());
			initLobbyPlayer ();
		}
	}


	public void OnDestroy(){
		Debug.Log ("CustomLobbyPlayer: Destroy");
		if (playerUI != null) {
			Destroy (playerUI);	
		}
		nextPid = pid;
	}




	//////////////////////////////////////Custom Methods//////////////////////////////////////////////

	private void initLobbyPlayer(){
		Debug.Log ("CustomLobbyPlayer: initLobbyPlayer");
		if (playerUI == null) {
			playerUI = Instantiate (PlayerUiPrefab, FindObjectOfType<Canvas> ()
				.GetComponentInChildren<VerticalLayoutGroup> ().transform);
		}
		Text[] playerUiElements = playerUI.GetComponentsInChildren<Text> ();
		playerUiElements [0].text = username;
		playerUiElements [1].text = pid.ToString();
		playerUiElements [2].text = ipAddress;
	}


	//////////////////////////////////////Commands//////////////////////////////////////////////

	[Command]
	public void CmdInitLobbyPlayer(string username, string ipAddress){
		pid = nextPid;
		nextPid++;
		this.ipAddress = ipAddress;
		this.username = username;
		Debug.Log ("CustomLobbyPlayer: CmdInitLobbyPlayer: " +
			"pid: " + pid.ToString() + ", ip: " + ipAddress + ", user: " + username
			+ ", next pid: " + nextPid.ToString());
	}

	//////////////////////////////////////SyncVar hooks//////////////////////////////////////////////

	private void initLobbyPlayerPid(Player.PlayerId newPid){
		pid = newPid;
		Debug.Log ("CustomLobbyPlayer: initLobbyPlayerPid");
		if (playerUI == null) {
			playerUI = Instantiate (PlayerUiPrefab, FindObjectOfType<Canvas> ()
				.GetComponentInChildren<VerticalLayoutGroup> ().transform);
		}
		Text[] playerUiElements = playerUI.GetComponentsInChildren<Text> ();
		playerUiElements [1].text = pid.ToString();

	}

	private void initLobbyPlayerUsername(string newUsername){
		username = newUsername;
		Debug.Log ("CustomLobbyPlayer: initLobbyPlayerUsername");
		if (playerUI == null) {
			playerUI = Instantiate (PlayerUiPrefab, FindObjectOfType<Canvas> ()
				.GetComponentInChildren<VerticalLayoutGroup> ().transform);
		}
		Text[] playerUiElements = playerUI.GetComponentsInChildren<Text> ();
		playerUiElements [0].text = username;

	}

	private void initLobbyPlayerIp(string newIp){
		ipAddress = newIp;
		Debug.Log ("CustomLobbyPlayer: initLobbyPlayerIp");
		if (playerUI == null) {
			playerUI = Instantiate (PlayerUiPrefab, FindObjectOfType<Canvas> ()
				.GetComponentInChildren<VerticalLayoutGroup> ().transform);
		}
		Text[] playerUiElements = playerUI.GetComponentsInChildren<Text> ();
		playerUiElements [2].text = ipAddress;

	}


}
