using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyScreen : NetworkBehaviour {

	//Lobby UI
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
	}
	
	[Command]
	public void CmdConnectPlayer(string username, string ipAddress, bool isHost){
		Debug.Log ("CmdConnectPlayer: isHost: " + isHost);
		RpcConnectPlayer(username, ipAddress, isHost, InitialGameState.nextPid);
		InitialGameState.nextPid++;
	}



	[ClientRpc]
	public void RpcConnectPlayer(string username, string ipAddress, bool isHost, Player.PlayerId pid){
		Debug.Log ("RpcConnectPlayer: isHost: " + isHost);
		switch (pid) {
		case Player.PlayerId.P1:{
				p1NameText.text = username;
				p1IP.text = ipAddress;
				p1RoleText.text = isHost ? "Host" : "Client";
				break;
			}
		case Player.PlayerId.P2:{
				p2NameText.text = username;
				p2IP.text = ipAddress;
				p2RoleText.text = isHost ? "Host" : "Client";
				break;
			}
		default:{
				NetworkClient.ShutdownAll ();
				break;
			}
		}
	}
}
