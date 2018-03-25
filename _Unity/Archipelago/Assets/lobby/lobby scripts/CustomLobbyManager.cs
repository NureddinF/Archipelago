using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CustomLobbyManager : NetworkLobbyManager {

	//State variables. Need to manually set isDebug/isHost for running from unity editor or PC build as there's no android part
	public bool isDebug = false;
	public bool isHost = false;
	public bool isSinglePlayer = false;

	// UI elements
	public GameObject searchUi;
	public GameObject lobbyUi;
	public Text JoiningText;
	public Text enteredIpAddress;

	//on failed connection the scene is reloaded so the state needs to be saved
	string ipStr = "";
	string joiningStr = "";
	private NetworkClient netClient = null;

	// Use this for initialization
	void Start () {
		//enteredIpAddress = searchUi.GetComponentInChildren<InputField>().textComponent;
		Debug.Log ("NetworkLobbyManager: Start");

		if (!isDebug) {
			string sceneName = AndroidWrapper.getAndroidSceneName ();
			InitialGameState.username = AndroidWrapper.getUsername ();
			switch(sceneName){
				case "play":{
					isSinglePlayer = true;
					break;	
				}
				case "host":{
					isHost = true;
					InitialGameState.HostIpAddr = AndroidWrapper.getIpAddr();
					break;	
				}
				case "join":{
					isHost = false;
					InitialGameState.ClientIpAddr = AndroidWrapper.getIpAddr();
					break;
				}
				default:{
					throw(new System.ArgumentException (
						"Game started with invlaid command. Must be one of: 'host','join', or 'play'"));
				}
			}
		}


		if (isSinglePlayer){
			Debug.Log ("NetworkLobbyManager: Start: isSinglePlayer");
			maxPlayers = 1;
			isHost = true;
		}


		if(isHost){
			Debug.Log ("NetworkLobbyManager: Start: isHost");
			networkAddress = InitialGameState.HostIpAddr;
			StartHost ();
		}
			

		InitialGameState.isHost = isHost;
	}


	public void initLobbyUi(){
		Debug.Log ("NetworkLobbyManager: initLobbyUi");

		searchUi.SetActive (false);
		lobbyUi.SetActive (true);

	}

	public void attemptConnection(){
		Debug.Log ("NetworkLobbyManager: attempting connection");
		// Get the entered string
		ipStr = enteredIpAddress.text;
		//remove whitespace
		ipStr = RemoveWhitespace(ipStr);
		//split string into bytes
		string[] ipBytesString = ipStr.Split ('.');

		//make sure the ip address if the correct format for IPv4 (X.X.X.X)
		if (ipBytesString.Length != 4) {
			Debug.Log ("bad length ('.' seperators)");
			return;
		}

		//make sure the ip addres is valid
		try{
			for (int i = 0; i < 4; i++) {
				byte  b = Byte.Parse(ipBytesString[i]);
			}
		} catch (Exception e){
			Debug.Log ("bad format: ipStr: " + ipStr + e.ToString());
			return;
		}
		Debug.Log ("valid Ip address");

		// Save state and attempt connection
		InitialGameState.HostIpAddr = ipStr;
		networkAddress = ipStr;
		JoiningText.text = "Joining...";

		if(netClient != null){
			netClient.Disconnect ();
		}

		netClient = StartClient ();
	}

	// https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string
	public static string RemoveWhitespace(string str) {
		return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
	}


	public string getClientIp(){
		return isHost ? InitialGameState.HostIpAddr : InitialGameState.ClientIpAddr;
	}

	public override void OnLobbyClientDisconnect (NetworkConnection conn){
		Debug.Log ("NetworkLobbyManager: OnLobbyClientDisconnect:");
		joiningStr = "Disconnected From Server";
		JoiningText.text = joiningStr;
	}

	public override void OnLobbyClientSceneChanged (NetworkConnection conn)
	{
		base.OnLobbyClientSceneChanged (conn);
		Debug.Log ("NetworkLobbyManager: OnLobbyClientSceneChanged:");
	}


	public override void OnLobbyServerPlayersReady (){
		lobbyUi.GetComponentInChildren<Button> ().interactable = true;
	}

	public void playerUnready(){
		lobbyUi.GetComponentInChildren<Button> ().interactable = false;
	}

	public void onStartGameClicked(){
		ServerChangeScene (playScene);
	}


	public void returnToAndroidmenu(){
		if (!isDebug) {
			AndroidWrapper.returnToAndroidMenu ();
		} else {
			Debug.Log ("Debug Mode enabled - not running on android device");
		}
	}


	public void setLobbyUi(GameObject lobbyUi){
		this.lobbyUi = lobbyUi;
	}

	public void setSearchUi(GameObject searchUi){
		this.searchUi = searchUi;
		enteredIpAddress = searchUi.transform.Find("InputField").Find("IP_text").GetComponent<Text> ();
		enteredIpAddress.text = ipStr;
		JoiningText = searchUi.transform.Find ("JoiningText").GetComponent<Text>();
		JoiningText.text = joiningStr;
	}

	// STEPS TO START A GAME:
	// 1) First scene loads when unity starts from android studio.
	//	  Get the option user selected: single player | host | join

	//    single player) Automatically jump through steps of starting a game as host with only 1 player
	//					 Don't have to display host screen but could if its easier


	//	  host) start host. This should load the lobby scene and display first player
	//			When second player joins display their information.
	//			When both players hit ready button enable start game button on host
	//			When host hits start game, start the game for all players
	//			Host should have option to disband lobby which returns everyone to android menu


	//	  Join) load connection menu (unique scene?). Player enters host ip and hits connect
	//			If player IP is valid attempt connection else give error message
	//			If connected load lobby scene and display both players info.
	//			If connection fails give error message and let user try again.
	//			While trying to connect give indication that this is happening
	//			Allow user to cancel connecting up to the point where they load
	//			Allow user to leave lobby once they've connected returning them to android menu
	//			Allow user to return to android menu from the screen where they enter IP address
	
}
