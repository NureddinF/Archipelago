using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerConnectionScript : NetworkBehaviour{

	public Player.PlayerId pid = Player.PlayerId.NEUTRAL;

	public GameObject gameplayPlayerPrefab;

	private GameObject gameplayPlayer;

	// Use this for initialization
	void Start (){
		Debug.Log("Started player connection script");
	}
	
	// Update is called once per frame
	void Update (){
	
	}


	//////////////////// Custom Methods ///////////////////////////////////////////
	public void initGameplayerPlayer(){
		if (isLocalPlayer) {
			CmdInitGameplayObject ();
		}
	}



	/////////////////// CLIENT SIDE METHODS ////////////////////////////////////// 
	public override void OnStartClient() {
		Debug.Log("PlayerConnectionScript: Started client.");
	}

	public void OnConnectedToServer() {
		Debug.Log("PlayerConnectionScript: Connected to server");
	}


	public void OnFailedToConnect(){
		Debug.Log("PlayerConnectionScript: Failed to connect to server");
	}

	public void OnDisconnectedFromServer(){
		Debug.Log("PlayerConnectionScript: Disconnected from server");
	}

	/////////////////// SERVER SIDE METHODS ////////////////////////////////////// 
	public override  void OnStartServer() {
		Debug.Log("PlayerConnectionScript: Started server");
	}

	public void OnPlayerConnected(){
		Debug.Log("PlayerConnectionScript: Player connected");
	}


	public void OnPlayerDisconnected(){
		Debug.Log("PlayerConnectionScript: Player disconnected");
	}

	/////////////////////////// Commands ////////////////////////////////////// 

	[Command]
	private void CmdInitGameplayObject (){
		// Instanceiate an instace of the gameplay player prefab
		gameplayPlayer = Instantiate (gameplayPlayerPrefab, transform);

		//Spawn the object on the network
		NetworkServer.SpawnWithClientAuthority (gameplayPlayer, connectionToClient);

		// Initialize the gameplay player object
		Player player = gameplayPlayer.GetComponent<Player> ();
		player.CmdSetPlayerId(pid);
		player.RpcStartWithAuthority ();
	}
}

