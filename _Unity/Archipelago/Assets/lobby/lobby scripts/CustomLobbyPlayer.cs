using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CustomLobbyPlayer : NetworkLobbyPlayer {

	public GameObject PlayerUiPrefab; //prefab of lobby UI for player info
	private GameObject playerUI; //reference to spawned version of player UI
	private Player.PlayerId pid;

	private static Player.PlayerId nextPid = Player.PlayerId.P1;

	// Use this for initialization
	void Start () {

	}


	public override void OnClientEnterLobby (){
		Debug.Log ("CustomLobbyPlayer: OnClientEnterLobby");

		if(hasAuthority){
			Debug.Log ("CustomLobbyPlayer: OnClientEnterLobby: hasAuthority");
			CmdInitLobbyPlayer ("User " + Random.Range(0,100));
		}
	}




	[Command]
	public void CmdInitLobbyPlayer(string username){
		playerUI  = Instantiate (PlayerUiPrefab);

		pid = nextPid;
		nextPid++;

		Text[] playerUiElements = playerUI.GetComponentsInChildren<Text> ();
		playerUiElements [0].text = username;
		playerUiElements [1].text = pid.ToString();
		playerUiElements [2].text = connectionToClient.address;

		NetworkServer.Spawn (playerUI);
		LinkToParent(playerUI,FindObjectOfType<Canvas> ().GetComponentInChildren<VerticalLayoutGroup> ().gameObject);
		Debug.Log ("CmdInitLobbyPlayer");

	}


	private static void LinkToParent(GameObject child, GameObject parent)
	{
		child.transform.parent = parent.transform;
		NetworkServer.SendToAll(SetParentMessage.SetParent, new SetParentMessage(child, parent));
	}

	[ClientRpc]
	public void RpcInitLobbyPlayer(int x){
		GameObject newPlayerUI = null;
		Debug.Log ("RpcInitLobbyPlayer: newPlayerUI: ");
		return;
		//+ newPlayerUI == null ? "" : newPlayerUI.name);
		playerUI = newPlayerUI;
		playerUI.transform.SetParent (FindObjectOfType<Canvas> ()
			.GetComponentInChildren<VerticalLayoutGroup> ().transform);
	}
}
