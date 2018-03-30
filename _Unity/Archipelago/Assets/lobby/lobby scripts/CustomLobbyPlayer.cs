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

	void Awake(){
		// Need to keep this object around when scene changes from lobby to gameplay to spawn the gameplayer player object
		DontDestroyOnLoad (gameObject);
	}

	// Use this for initialization
	void Start () {
		Debug.Log("CustomLobbyPlayer: Start");
		if(isLocalPlayer){
			//This is the player object for the local client, initialize it
			Debug.Log ("CustomLobbyPlayer: Start: isLocalPlayer");
			FindObjectOfType<CustomLobbyManager> ().initLobbyUi ();
			//sync UI across all clients
			CmdInitLobbyPlayer (InitialGameState.username, FindObjectOfType<CustomLobbyManager>().getClientIp());
			Debug.Log ("CustomLobbyPlayer: isSinglePlayer=" + FindObjectOfType<CustomLobbyManager> ().isSinglePlayer);
			if (FindObjectOfType<CustomLobbyManager> ().isSinglePlayer) {
				// For single player game there's no need for the lobby
				SendReadyToBeginMessage ();
			}
			Debug.Log ("CustomLobbyPlayer: Start: isLocalPlayer: "
				+ "username: " + username + ", ipAddress: " + ipAddress
				+ "player: " + pid.ToString());

		} else {
			//Not the local player, set up UI Object it to display data from the other client
			Debug.Log ("CustomLobbyPlayer: Start: NOT isLocalPlayer: "
				+ "username: " + username + ", ipAddress: " + ipAddress
				+ "player: " + pid.ToString());
			initLobbyPlayer ();
		}
	}


	public void OnDestroy(){
		Debug.Log ("CustomLobbyPlayer: Destroy");
		//Player lobby object being destryed (player DC'd), remove UI object for that player
		if (playerUI != null) {
			Destroy (playerUI);	
		}
		//Reset next player ID next player that joins get assigned ID correctly
		nextPid = pid;
	}




	//////////////////////////////////////Custom Methods//////////////////////////////////////////////

	private void initLobbyPlayer(){
		// Set up Lobby UI for client when it connects
		Debug.Log ("CustomLobbyPlayer: initLobbyPlayer");
		if (playerUI == null) {
			Canvas c = FindObjectOfType<Canvas> ();
			Transform lui = c.transform.Find ("LobbyUi");
			Transform correctParent = lui.Find ("ConnectionStatus");
			Debug.Log ("CustomLobbyPlayer: initLobbyPlayer: parent: " + correctParent == null ? "NULL" : correctParent.name);
			playerUI = Instantiate (PlayerUiPrefab, correctParent);
		}
		Toggle toggle = playerUI.GetComponentInChildren<Toggle> ();
		toggle.isOn = readyToBegin;

		Text[] playerUiElements = playerUI.GetComponentsInChildren<Text> ();
		playerUiElements [0].text = username;
		playerUiElements [1].text = pid.ToString();
		playerUiElements [2].text = ipAddress;
	}


	//////////////////////////////////////Commands//////////////////////////////////////////////

	[Command]
	public void CmdInitLobbyPlayer(string username, string ipAddress){
		// Syncronize player parameters on the server
		pid = nextPid;
		nextPid++;
		this.ipAddress = ipAddress;
		this.username = username;
		Debug.Log ("CustomLobbyPlayer: CmdInitLobbyPlayer: " +
			"pid: " + pid.ToString() + ", ip: " + ipAddress + ", user: " + username
			+ ", next pid: " + nextPid.ToString());
	}

	//////////////////////////////////////SyncVar hooks//////////////////////////////////////////////

	// Sync UI for player ID
	private void initLobbyPlayerPid(Player.PlayerId newPid){
		pid = newPid;
		if(FindObjectOfType<CustomLobbyManager>().isSinglePlayer){
			return;
		}
		Debug.Log ("CustomLobbyPlayer: initLobbyPlayerPid");
		if (playerUI == null) {
			initUi ();
		}
		Text[] playerUiElements = playerUI.GetComponentsInChildren<Text> ();
		playerUiElements [1].text = pid.ToString();

	}

	// Sync UI for player username
	private void initLobbyPlayerUsername(string newUsername){
		username = newUsername;
		if(FindObjectOfType<CustomLobbyManager>().isSinglePlayer){
			return;
		}
		Debug.Log ("CustomLobbyPlayer: initLobbyPlayerUsername");
		if (playerUI == null) {
			initUi ();
		}
		Text[] playerUiElements = playerUI.GetComponentsInChildren<Text> ();
		playerUiElements [0].text = username;

	}

	// Sync UI for player IP address
	private void initLobbyPlayerIp(string newIp){
		ipAddress = newIp;
		if(FindObjectOfType<CustomLobbyManager>().isSinglePlayer){
			return;
		}
		Debug.Log ("CustomLobbyPlayer: initLobbyPlayerIp");
		if (playerUI == null) {
			initUi ();
		}
		Text[] playerUiElements = playerUI.GetComponentsInChildren<Text> ();
		playerUiElements [2].text = ipAddress;

	}

	// If a player UI isn't initialized when the parameters get updated create one
	private void initUi(){
		if(!isLocalPlayer){
			//return;
		}


		Canvas canvas = FindObjectOfType<Canvas> ();
		if(canvas == null){
			return;
		}

		VerticalLayoutGroup lobbyPlayerUI = canvas.GetComponentInChildren<VerticalLayoutGroup> ();
		if (lobbyPlayerUI == null) {
			return;
		}
		playerUI = Instantiate (PlayerUiPrefab, lobbyPlayerUI.transform);
		
		Toggle toggle = playerUI.GetComponentInChildren<Toggle>();
		toggle.onValueChanged.AddListener (onReadyChecked);
		toggle.interactable = true;
	}


	public void onReadyChecked(bool ready){
		Debug.Log ("CustomLobbyPlayer: onReadyChecked: ready: " + ready);

		if(!isLocalPlayer){
			// Don't let people unready others
			return;
		}

		// Let server know this player is ready to start the game
		if (ready) {
			SendReadyToBeginMessage ();
		} else {
			SendNotReadyToBeginMessage ();
		}
	}
		
	// Client toggled if they were ready
	public override void OnClientReady (bool clientReady){
		if (playerUI != null) {
			// Sync checkmark for players readying across clients
			Toggle toggle = playerUI.GetComponentInChildren<Toggle> ();
			toggle.isOn = clientReady;
		}

		//Make sure start game button disalbes on host if player isn't ready
		if(!clientReady){
			FindObjectOfType<CustomLobbyManager> ().playerUnready ();
		}
	}
		
}
