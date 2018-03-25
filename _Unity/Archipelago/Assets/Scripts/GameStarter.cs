using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameStarter : MonoBehaviour {

	// Local network manager
	private NetworkManager netMan;

	// Use this for initialization
	void Start () {
		//Initialize network manager
		netMan = FindObjectOfType<NetworkManager> ();
		netMan.networkAddress = InitialGameState.HostIpAddr;

		//Start client or host
		if(InitialGameState.isHost){
			//Start the host
			netMan.StartHost();
		} else {
			//Start the client
			netMan.StartClient();
		}
	}
	

	public void onStartGameClicked(){
		Debug.Log("Starting gameplay!");
		netMan.ServerChangeScene (SceneLoader.gameplaySceneUnityName);
	}

}
