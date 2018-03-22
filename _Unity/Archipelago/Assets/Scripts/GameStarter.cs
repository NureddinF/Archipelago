using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameStarter : NetworkBehaviour {

	private NetworkManager netMan;


	private Text p1NameText;
	private Text p1RoleText;
	private Text p1IP;

	private Text p2NameText;
	private Text p2RoleText;
	private Text p2IP;

	// Use this for initialization
	void Start () {

		//Initialize lobby UI
		p1NameText = GameObject.Find("Player1 Name").GetComponent<Text>();
		p1RoleText = GameObject.Find("Player1 Role").GetComponent<Text>();
		p1IP = GameObject.Find("Player1 IP").GetComponent<Text>();
		p2NameText = GameObject.Find("Player2 Name").GetComponent<Text>();
		p2RoleText = GameObject.Find("Player2 Role").GetComponent<Text>();
		p2IP = GameObject.Find("Player2 IP").GetComponent<Text>();


		//Initialize network manager
		netMan = FindObjectOfType<NetworkManager> ();
		netMan.networkAddress = InitialGameState.hostIpAddr;

		//Start client or host
		if(InitialGameState.isHost){
			//TODO: display player's username
			p1IP.text = InitialGameState.hostIpAddr;

			//Start the host
			netMan.StartHost();
		} else {
			//Start the client
			netMan.StartClient();
		}
	}
	

	public void onStartGameClicked(){
		//TODO: load game scene with network manager
		Debug.Log("Starting gameplay!");

		netMan.ServerChangeScene (SceneLoader.gameplaySceneUnityName);
	}


	/////////////////// CLIENT SIDE METHODS ////////////////////////////////////// 
	public void OnConnectedToServer() {
		Debug.Log("Connected to server");
	}


	public void OnFailedToConnect(){
		Debug.Log("Failed to connect to server");
	}

	public void OnDisconnectedFromServer(){
		Debug.Log("Disconnected from server");
	}

	/////////////////// SERVER SIDE METHODS ////////////////////////////////////// 
	public void OnPlayerConnected(){
		Debug.Log("Player connected");
	}


	public void OnPlayerDisconnected(){
		Debug.Log("Player disconnected");
	}



}
