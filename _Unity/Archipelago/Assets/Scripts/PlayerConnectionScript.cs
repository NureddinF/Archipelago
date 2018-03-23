using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerConnectionScript : NetworkBehaviour
{

	// Use this for initialization
	void Start (){
		Debug.Log("Started player connection script");
	}
	
	// Update is called once per frame
	void Update (){
	
	}


	/////////////////// CLIENT SIDE METHODS ////////////////////////////////////// 
	public override void OnStartClient() {
		Debug.Log("Started client.");
		Debug.Log (" My IP: "+ NetworkManager.singleton.networkAddress);
		FindObjectOfType<LobbyScreen> ().CmdConnectPlayer (
			InitialGameState.username,
			NetworkManager.singleton.networkAddress,
			InitialGameState.isHost);
	}

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
	public override  void OnStartServer() {
		Debug.Log("Started server");
	}

	public void OnPlayerConnected(){
		Debug.Log("Player connected");
	}


	public void OnPlayerDisconnected(){
		Debug.Log("Player disconnected");
	}

}

