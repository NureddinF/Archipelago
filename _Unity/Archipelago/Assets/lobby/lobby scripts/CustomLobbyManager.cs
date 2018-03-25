using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomLobbyManager : NetworkLobbyManager {

	public bool isDebug = false;
	public bool isHost = false;
	public bool isSinglePlayer = false;

	// Use this for initialization
	void Start () {
		Debug.Log ("NetworkLobbyManager: Start");
		if (!isDebug) {
			string sceneName = AndroidWrapper.getAndroidSceneName ();
			switch(sceneName){
				case "play":{
					isSinglePlayer = true;
					break;	
				}
				case "host":{
					isHost = true;
					InitialGameState.hostIpAddr = AndroidWrapper.getIpAddr();
					break;	
				}
				case "join":{
					isHost = false;
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
			networkAddress = InitialGameState.hostIpAddr;
			StartHost ();
		} else {
			Debug.Log ("NetworkLobbyManager: Start: isClient");
			networkAddress = InitialGameState.hostIpAddr;
			StartClient ();
		}
			

		InitialGameState.isHost = isHost;
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
