using UnityEngine;
using System.Collections;

public class PlayerConnectionScript : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
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

