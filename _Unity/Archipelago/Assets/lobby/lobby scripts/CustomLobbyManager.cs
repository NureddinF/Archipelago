using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

public class CustomLobbyManager : NetworkLobbyManager {

	//State variables. Need to manually set isDebug for running from unity editor or PC build as there's no android part
	public bool isDebug = false;
	public bool isHost = false;
	public bool isSinglePlayer = false;
	private bool sceneLoadedForLocalPlayer = false;
	private bool gameplayerPlayerObjectInitialised = false;

	// UI elements
	public GameObject exitButton;
	public GameObject loadingText;
	private GameObject searchUi;
	private GameObject lobbyUi;
	public Text JoiningText;
	public Text enteredIpAddress;

	//on failed connection the scene is reloaded so the state needs to be saved
	string ipStr = "";
	string joiningStr = "";
	private NetworkClient netClient = null;

	// Use this for initialization
	void Start () {
		Debug.Log ("NetworkLobbyManager: Start");

		//Don't make calls to android native code if testing in unity editor
		if (!isDebug) {
			//Running on android device, get needed parameters from android activity
			string startCommand = AndroidWrapper.getAndroidStartCommand ();
			InitialGameState.username = AndroidWrapper.getUsername ();
			switch(startCommand){
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

		//Configure parameters to automatically start gameplay if game is started as single players
		if (isSinglePlayer){
			Debug.Log ("NetworkLobbyManager: Start: isSinglePlayer");
			minPlayers = 1;
			maxPlayers = 1;
			isHost = true;
		}


		InitialGameState.isHost = isHost;

		//Start lobby screen as host
		if(isHost){
			Debug.Log ("NetworkLobbyManager: Start: isHost");
			networkAddress = InitialGameState.HostIpAddr;
			StartHost ();
		}
	}

	//Load the UI when the game starts
	public void initLobbyUi(){
		Debug.Log ("NetworkLobbyManager: initLobbyUi");
		if (isSinglePlayer) {
			//don't load lobby UI, instead just start the game
			searchUi.SetActive (false);
			lobbyUi.SetActive (false);
			exitButton.SetActive(false);
			loadingText.SetActive(true); //Game takes a send to start
		} else {
			searchUi.SetActive (false);
			lobbyUi.SetActive (true);
		}
	}


	// Callback for "connect" button after user enters hosts ip address when trying to join a game
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
				Byte.Parse(ipBytesString[i]);
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

		//if there is a connection in progress cancel it before starting new one
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

	public override void OnLobbyClientSceneChanged (NetworkConnection conn){
		base.OnLobbyClientSceneChanged (conn);
		Debug.Log ("NetworkLobbyManager: OnLobbyClientSceneChanged");
		sceneLoadedForLocalPlayer = true;
		initGameplayPlayerObject ();
	}

	public void initGameplayPlayerObject(){
		if (!sceneLoadedForLocalPlayer || gameplayerPlayerObjectInitialised) {
			// scene hasn't loaded yet, wiat for that OR
			// already spawned player object for the local player, don't do it again
			return;
		}

		// Cycle through player objects until we find the local player
		PlayerConnectionScript[] playerConnectionObjects = FindObjectsOfType<PlayerConnectionScript>();
		foreach (PlayerConnectionScript playerConnectionObject in playerConnectionObjects) {
			if (playerConnectionObject.isLocalPlayer) {
				// Found local player, spawn gameplay player object
				gameplayerPlayerObjectInitialised = true;
				playerConnectionObject.initGameplayerPlayer ();
				return;
			}
		}
	}

	public override void OnLobbyServerPlayersReady (){
		Debug.Log ("OnLobbyServerPlayersReady");
		//All players are ready to start
		//Check if this is a single player or multiplayer game
		if (isSinglePlayer) {
			//start game imediately
			onStartGameClicked ();
		} else {
			//enable button to let host start game
			lobbyUi.GetComponentInChildren<Button> ().interactable = true;
		}
	}

	public void playerUnready(){
		//disable start game button if someone unreadies
		lobbyUi.GetComponentInChildren<Button> ().interactable = false;
	}

	public void onStartGameClicked(){
		//When start game button is clicked load the gameplay scene on all clients
		ServerChangeScene (playScene);
	}

	// back button clicked
	public void returnToAndroidmenu(){
		if (!isDebug) {
			AndroidWrapper.returnToAndroidMenu ();
		} else {
			Debug.Log ("Debug Mode enabled - not running on android device");
		}
	}

	// When scene loads hook up UI components (lobby screen)
	public void setLobbyUi(GameObject lobbyUi){
		this.lobbyUi = lobbyUi;
	}

	// When scene loads hook up UI components (join game screen)
	public void setSearchUi(GameObject searchUi){
		this.searchUi = searchUi;
		enteredIpAddress = searchUi.transform.Find("InputField").Find("IP_text").GetComponent<Text> ();
		enteredIpAddress.text = ipStr;
		JoiningText = searchUi.transform.Find ("JoiningText").GetComponent<Text>();
		JoiningText.text = joiningStr;
	}


	// Called when gameplay scene is loaded. Used to initialize Player Gameplayobject with parameters from the lobby
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer){
		bool baseRet = base.OnLobbyServerSceneLoadedForPlayer (lobbyPlayer, gamePlayer);
		Debug.Log ("NetworkLobbyManager: OnLobbyServerSceneLoadedForPlayer");
		PlayerConnectionScript gamePlayerScript = gamePlayer.GetComponent<PlayerConnectionScript> ();
		CustomLobbyPlayer lobbyPlayerScript = lobbyPlayer.GetComponent<CustomLobbyPlayer> ();

		gamePlayerScript.pid = lobbyPlayerScript.pid;

		return baseRet;
	}

	/// <summary>
	/// ////////////////////////////////////
	/// </summary>
	/// <param name="conn">Conn.</param>





	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId)
	{
		Debug.LogWarning ("NetworkLobbyManager: OnServerAddPlayer: playerControllerId=" + playerControllerId);
		if (SceneManager.GetSceneAt(0).name != this.lobbyScene)
			return;
		int num = 0;
		using (List<PlayerController>.Enumerator enumerator = conn.playerControllers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsValid)
					++num;
			}
		}
		if (num >= this.maxPlayersPerConnection)
		{
			if (LogFilter.logWarn)
				Debug.LogWarning((object) "NetworkLobbyManager no more players for this connection.");
			EmptyMessage emptyMessage = new EmptyMessage();
			conn.Send((short) 45, (MessageBase) emptyMessage);
		}
		else
		{
			byte slot = this.FindSlot();
			if ((int) slot == (int) byte.MaxValue)
			{
				if (LogFilter.logWarn)
					Debug.LogWarning((object) "NetworkLobbyManager no space for more players");
				EmptyMessage emptyMessage = new EmptyMessage();
				conn.Send((short) 45, (MessageBase) emptyMessage);
			}
			else
			{
				GameObject player = (GameObject)UnityEngine.Object.Instantiate(this.lobbyPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
				NetworkLobbyPlayer component = player.GetComponent<NetworkLobbyPlayer>();
				component.slot = slot;
				this.lobbySlots[(int) slot] = component;
				NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
			}
		}
	}

	private byte FindSlot()
	{
		for (byte index = 0; (int) index < this.maxPlayers; ++index)
		{
			if ((UnityEngine.Object) this.lobbySlots[(int) index] == (UnityEngine.Object) null)
				return index;
		}
		return byte.MaxValue;
	}

	public override void OnClientSceneChanged(NetworkConnection conn){
		Debug.LogWarning ("NetworkLobbyManager: OnClientSceneChanged: connectionID" + conn.connectionId);
		if (SceneManager.GetSceneAt(0).name == lobbyScene)
		{
			if (this.client.isConnected)
				this.CallOnClientEnterLobby();
		}
		else
			this.CallOnClientExitLobby();
		OnClientSceneChangedBase(conn);
		this.OnLobbyClientSceneChanged(conn);
	}

	private void OnClientSceneChangedBase(NetworkConnection conn){
		Debug.LogWarning ("NetworkLobbyManager: OnClientSceneChangedBase: connectionID" + conn.connectionId);
		bool flag1 = ClientScene.localPlayers.Count == 0;
		bool flag2 = false;
		using (List<PlayerController>.Enumerator enumerator = ClientScene.localPlayers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if ((UnityEngine.Object) enumerator.Current.gameObject != (UnityEngine.Object) null)
				{
					flag2 = true;
					break;
				}
			}
		}
		if (!flag2)
			flag1 = true;
		if (!flag1)
			return;
		ClientScene.AddPlayer((short) 0);
	}

	public override void OnClientConnect(NetworkConnection conn){
		Debug.LogWarning ("NetworkLobbyManager: OnClientConnect: connectionID" + conn.connectionId);

		bool addPlayer = false;
		if (ClientScene.localPlayers.Count == 0)
		{
			// no players exist
			addPlayer = true;
		}

		bool foundPlayer = false;
		foreach (var playerController in ClientScene.localPlayers)
		{
			if (playerController.gameObject != null)
			{
				foundPlayer = true;
				break;
			}
		}
		if (!foundPlayer)
		{
			// there are players, but their game objects have all been deleted
			addPlayer = true;
		}
		if (addPlayer)
		{
			ClientScene.AddPlayer(conn, 0);
		}
		//ClientScene.AddPlayer(conn, 0);
	}









	/*
	public void OnClientSceneChanged(NetworkConnection conn){
		Debug.Log ("NetworkLobbyManager: OnClientSceneChanged: connectionID" + conn.connectionId);
		string loadedSceneName = SceneManager.GetSceneAt(0).name;
		if (loadedSceneName == lobbyScene)
		{
			if (client.isConnected)
				CallOnClientEnterLobby();
		}
		else
		{
			CallOnClientExitLobby();
		}

		/// This call is commented out since it causes a unet "A connection has already been set as ready. There can only be one." error.
		/// More info: http://answers.unity3d.com/questions/991552/unet-a-connection-has-already-been-set-as-ready-th.html
		//base.OnClientSceneChanged(conn);
		OnLobbyClientSceneChanged(conn);
	}*/
		

	private void CallOnClientEnterLobby(){
		this.OnLobbyClientEnter();
		foreach (NetworkLobbyPlayer lobbySlot in this.lobbySlots)
		{
			if (!((UnityEngine.Object) lobbySlot == (UnityEngine.Object) null))
			{
				lobbySlot.readyToBegin = false;
				lobbySlot.OnClientEnterLobby();
			}
		}
	}

	private void CallOnClientExitLobby()
	{
		this.OnLobbyClientExit();
		foreach (NetworkLobbyPlayer lobbySlot in this.lobbySlots)
		{
			if (!((UnityEngine.Object) lobbySlot == (UnityEngine.Object) null))
				lobbySlot.OnClientExitLobby();
		}
	}
}
