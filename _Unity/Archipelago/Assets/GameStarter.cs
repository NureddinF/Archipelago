using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameStarter : MonoBehaviour {

	private NetworkManager netMan;

	// Use this for initialization
	void Start () {

		netMan = FindObjectOfType<NetworkManager> ();
		netMan.networkAddress = InitialGameState.hostIpAddr;

		//TODO: start client or host
		if(InitialGameState.isHost){
			netMan.StartHost();
		} else {
			netMan.StartClient();
		}
	}
	

	public void onStartGameClicked(){
		//TODO: load game scene with network manager
		Debug.Log("Starting gameplay!");

		netMan.ServerChangeScene (SceneLoader.gameplaySceneUnityName);
	}
}
