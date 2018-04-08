# Archipelago
Archipelago is a tile based mobile strategy game where 2 players connect via a network connection and compete for territory with the goal of reaching a predetermined sum of gold before their opponent does. Archipelago takes advantage of device storage to save player profile information. Enjoy upbeat and pleasant background music while you play; so we recommend using earphones or turning on sound while playing. Each captured tile adds to the total gold being earned. Players can build worker units to capture new tiles and build new buildings, which helps to both defend their territory as well as produce more gold. In addition players can also train warriors to battle the opponent head on to expand their territory.
## Libraries
Provide a list of **ALL** the libraries you used for your project. Example:

**google-gson:** Gson is a Java library that can be used to convert Java Objects into their JSON representation. It can also be used to convert a JSON string to an equivalent Java object. Source [here](https://github.com/google/gson)

**Unity version: 2017.3.0f3(MAC) 2017.3.1f1(PC)** Unity is a cross-platform game engine, used to develop 3D and 2D video games. Source [here](https://unity3d.com/)

## Installation Notes
Installation instructions for markers.

## Code Examples
You will encounter roadblocks and problems while developing your project. Share 2-3 'problems' that your team solved while developing your project. Write a few sentences that describe your solution and provide a code snippet/block that shows your solution. Example:

**Problem 1: Determining what touch actions to preform**

To determine wheather the touch on the screen is trying to zoom, pan the camera or select and object we created the following if, else - if block. It would start by checking if there is a touch on the screen and then check the phase of the touch. If the touch just began then it would store the touch and check wheater the touch is over a UI element which would return true or false. Then a check is made if the touch has moved, if true it would call on the pan camera method. Next we have a check to determine if the user is trying to zoom-in/out, it check if there are two touches on the screen and then calculate the center point of the two touches so the camera would zoom into the middle of the pinch.
```
// Handle clicking
		if (Input.touchCount == 0) {
			//For Debugging on PC
			if(Input.GetMouseButtonDown(0)){
				canPerformSelection = true;
				clickPos = Input.mousePosition;
			}
			// Check input status
			if(canPerformSelection){
				// Check if anything was clicked
				canPerformSelection = false;
				doSelection ();
			}
		}
		//panning
		else if (Input.touchCount == 1) { //one touch on screen

//			https://answers.unity.com/questions/517529/pan-camera-2d-by-touch.html , setting the boundary
//			https://answers.unity.com/questions/813224/move-camera-by-drag.html , creating a drag a zoom

			Touch singleTouch = Input.GetTouch (0); //stores touch
			if (singleTouch.phase == TouchPhase.Began) { //checks touch phase
				//gets touch position
				clickPos = singleTouch.position;
				//If this touch is not over a UI it might be for selecting a hex
				canPerformSelection = !EventSystem.current.IsPointerOverGameObject(singleTouch.fingerId);
			} else if (singleTouch.phase == TouchPhase.Moved) { //check phase if moved
				panCamera(singleTouch.position, clickPos);
				clickPos = singleTouch.position;
			} else if(singleTouch.phase == TouchPhase.Ended){ //touch phase ended
				Debug.Log("Ended: "+singleTouch.phase);
			}


		}

		//Zoom
		else if (Input.touchCount == 2) { //Checks for 2 touches on the screen

			//Get the touches
			Touch touchZero = Input.GetTouch(0); //stores the touches
			Touch touchOne = Input.GetTouch(1);

			// Check if a touch ended
			bool touchZeroEnded = touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled;
			bool touchOneEnded = touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled;

			//checks touch phase
			if (touchZeroEnded && !touchOneEnded) {
				// Need to make sure panning works smoothly after leaving this state
				clickPos = touchOne.position; 
			} else if (touchOneEnded && !touchZeroEnded){
				// Need to make sure panning works smoothly after leaving this state
				clickPos = touchZero.position; 
			} else if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began) {
				// New touch, find center point
				centerPoint = touchZero.position + (touchOne.position - touchZero.position)/2f;

			} else if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved) { //check phase if moved
				// At least one finger moved, zoom camera towards center of touches
				// Save starting world point for focused zoom on center of pinch
				Vector3 oldCenterWorldPoint = main.ScreenToWorldPoint(centerPoint);

				// Zoom camera
				pinchZoomCamera(touchZero, touchOne);
				// Pan camera so it doesn't just zoom on center of screen
				Vector3 newTouchCenterPoint = touchZero.position + (touchOne.position - touchZero.position)/2f;
				Vector3 diffBetweenCameraAndCenter = main.ScreenToWorldPoint(newTouchCenterPoint) - oldCenterWorldPoint;
				main.transform.position = clampNewCameraPos (main.transform.position - diffBetweenCameraAndCenter);

				// Update state
				centerPoint = newTouchCenterPoint;
			}
		}
```

**Problem 2: Merging Android Studio and Unity projects**

The initial set of menus with the login screen and registration was done in android studio (Java activities). The actual gameplay for the game was doen in unity (C# scripts). We had to find a way to allow these seperate parts of the app to pass information to one another to allow Unity to know if the game was being played as single player or one of the multiplayer options. 

To do this we had to build the unity project as android library and import it into the android studio project. This resulted in two modules in the project. We then had to modify the gradle for the unity module to make it a library:
```
apply plugin: 'com.android.library'



```

Next we had to add the unity library to the main app's gradle:

```
dependencies {
    implementation fileTree(include: ['*.jar'], dir: 'libs')
    implementation 'com.android.support:appcompat-v7:26.1.0'
    implementation 'com.android.support.constraint:constraint-layout:1.0.2'
    implementation 'com.android.support:wear:26.1.0'
    testImplementation 'junit:junit:4.12'
    androidTestImplementation 'com.android.support.test:runner:1.0.1'
    androidTestImplementation 'com.android.support.test.espresso:espresso-core:3.0.1'
    compileOnly 'com.google.android.wearable:wearable:2.2.0'
    implementation project(':Archiplegao_U')
    compile 'com.android.support:multidex:1.0.3'
}

```

Exporting the unity project automatically generates an Activity to represent it. Our android studio code launched the unity part by launching the unity activity with an intent. To pass addtional information to the unity part, Extra's are added to the intent:

```
private class GameLauncher implements View.OnClickListener {

        private final String startCommand;

        public GameLauncher(String startCommand){
            this.startCommand = startCommand;
        }

        @Override
        public void onClick(View view) {
            soundPool.play(soundId,1,1,0,0,1);
            Intent launchIntent = new Intent(getApplicationContext(), UnityPlayerActivity.class);
            if (launchIntent != null) {
                String username = SharedPreferenceUtils.getString(MainActivity.this,"username","Player");
                launchIntent.putExtra("username", username);
                launchIntent.putExtra("startCommand", startCommand);
                launchIntent.putExtra("ipaddr", getIpAddr());
                startActivityForResult(launchIntent, PLAY_GAME);
            } else {
                Toast.makeText(getApplicationContext(), "Could not find game APK", Toast.LENGTH_SHORT).show();
            }
        }
    }

```

In the unity activity it reads the intent data and then stores it in the activity while providing getters to retrieve it:

```
    @Override protected void onCreate(Bundle savedInstanceState)
    {
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        super.onCreate(savedInstanceState);
        startCommand = getIntent().getStringExtra("startCommand");
        ipAddr = getIntent().getStringExtra("ipaddr");
        username = getIntent().getStringExtra("username");
        getWindow().setFormat(PixelFormat.RGBX_8888); // <--- This makes xperia play happy

        mUnityPlayer = new UnityPlayer(this);
        setContentView(mUnityPlayer);
        mUnityPlayer.requestFocus();
    }
    
    public String getStartCommand(){
        return startCommand;
    }

    public String getIpAddr(){
        return ipAddr;
    }

    public String getUsername(){
        return username;
    }


```

When the unity code exectus it uses static c# functions to make native calls to the java getters:

```
	public static string getAndroidStartCommand(){

		AndroidJavaObject currentActivity = getUnityActivity();

		return currentActivity.Call<string>("getStartCommand");
	}
	
	private static AndroidJavaObject getUnityActivity(){
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

		return currentActivity;
	}
	

```

When the unity code wants to pass information back to android studio it can do this by making other native calls to the java activity. The following closes the game and returns to the menu screen when the player has won:

```
	public static void winAction(){
		AndroidJavaObject currentActivity = getUnityActivity();

		currentActivity.Call("win");
	}
	
	public void win() {
        	finish();
    	}

```

It was found that when unity project closes it doesn't do it cleanly (letting all its threads finish and join or something like that). Instead, when it closes it issues a "kill -9" on its process ID. This resulted in the entire app closing, not just the untiy part, when the game returned to android studio code. This was fixed by editting the unity android manifest to start the unity section in a different process:

```
<application android:theme="@style/UnityThemeSelector" android:icon="@drawable/app_icon" 			android:label="@string/app_name" android:isGame="true" android:banner="@drawable/app_banner"
      android:process=":Archipelago_U">

```

We also had to edit the android manifest to remove the intent filter on the generated activity. If this was not done then the app would have two icons on the device allowing the user to either start from the login screen or go to the game screen directly.

**Problem 3: Multiplayer - Not Enough Money**

When the game was changed to be multiplayer, it had to be redesigned to have state be maintained across server and clients. One example of this was when the user goes to build a building and they don't have enough money we wanted the cost to flash red. To execute this a series of steps are needed: they have to:
 - Button clieck triggers an on click listener:
 ```
	price[count].text = b.getCost ().ToString (); 
	go.GetComponent<Button> ().onClick.AddListener (() => {
		actionBuild (selectedHex.gameObject , b.buildingId, b.getCost());
	});
 ```
 - The client makes a requrest to the server telling it all the needed information. This is done using Untiy's High Level Networking API, specifically a command. When a command is called it is always executed on the server regardless of where it is called from. The parameters passed to the function have to be primitive types or networked objects (have the Network ID component and have been spawned correctly by the server).
 
 ```
 	void actionBuild(GameObject tile, Building.BuildingType buildingId, float cost){
		if (hasAuthority) {
			int index = 0;
			for (int i = 0; i < price.Length; i++) {
				if (price [i].name.ToString ().Equals (buildingId.ToString ())) {
					index = i;

				}
			}
			CmdTileActionBuild (tile, buildingId, cost, index);
			Debug.Log ("Count: " + index);
			Debug.Log ("price text: " + price [index].text);
		}

	}
 
 ```
 
 - The server checks cost of building and the players current money. If the player doesn't have enough it sends a failure message back to the client. This is done using a Remote Proceedure Call (RPC). The RPC is called on the server and executes on all the clients (each client has their own instance of the networked object). RPCs have the same restriction on parameters as commands.
 
 ```
 	// Command to build a building
	[Command]
	void CmdTileActionBuild(GameObject tile, Building.BuildingType buildingId, float cost, int count){
		float totalGold = GetComponent<Player> ().getCurrentMoney ();
		if(totalGold < cost) {
			RpcActionBuildFailed (count);
			Debug.Log ("INSUFFICENT FUNDS");
		}
		else {
			tile.GetComponent<Hex> ().CmdSetBuilding (buildingId);
			GetComponent<Player> ().removeMoney (cost);
			//Refresh hex menu's values to display these changes
			RpcRefreshUIValues ();
		}
	}
 ```

- The first thing the RPC does is check if it has authority. This is because each client has a copy of every player's gameobject and the RPC will execute on all clents for the player object that made the inital request. However, we don't want all players UI to flash thus we check if this player object is the authoritative player for this client (there will only be one authoritative player per client). Now that we know this is the client who made the the requrest we can start a coroutine to flash the UI:

```
	[ClientRpc]
	private void RpcActionBuildFailed(int index){
		if (hasAuthority && price != null) {
			//starts Coroutine to to ge the color to flash red
			StartCoroutine(ActionBuildFailed (Color.red, index));
		}
	}

	IEnumerator ActionBuildFailed(Color color, int index){
		//Changes the color to red
		price [index].color = color;
		//waits 
		yield return new WaitForSeconds (0.5f);
		//changes color back to the originial
		price [index].color = new Color (0, 0.75f, 0);
	}
```


## Feature Section
### Local Storage
- Device storage is utilized through the following: the remember me function created on Login and a user's login credentials are saved and are loaded when user opens application.
### Touch
- To utilize a touchscreen device's touch features we added a pinch to zoom touch gesture, drag to pan camera touch gesture, and a single touch/tap/click to select.
### Sound
- Background music for Android Studio and Unity and sound effects found in Android Studio. Sound options can be turned off inside the settings menu in Android Studio.
### Networking
- Lobby based multiplayer created for users to play together online. A player can Host the game or join a game. If a player hosts a game their IP address is used by player 2 to join their game.

## Final Project Status
The final status of our project is positive. Archipelago is working properly and features all minimal and expected functionality we originally set out to achieve. Including this we were able to achieve both of our bonus functionality goals. Background and sound effects are included in the Android Studio portion of our application and background music is found in the Unity portion. Multiplayer is working, allowing 2 players to connect via a network connection using the Host's IP address.

During the 3 months of development we were able to achieve a lot and inevitably created a working fun game. Given more time for development we believe we could have improved our game even more by adding a great deal of visual and functional polish. The following are sections of the game we would have liked to further improve upon:

**Multiplayer** - To make multiplayer more functional and to help players play with more people we would have liked add matchmaking. Due to time constraints and limited knowledge regarding the subject we were unable to implement this in the original development period. Matchmaking would have made playing with a friend easier as well creating the ability to play with people you don't even know.

**Music and Sound Effects** - Given more time and having some more experience with sound production/engineering it would have been nice to add more sound to add another dimension to the game. Better background sound and sound effects for actions may have created a more immersive experience for players as well as adding character to the game. This includes for example having the background music transition to project different moods or moments in the game. Due to lack of experience in the area of sound design and the limited time frame we were unable to add these in the initial development period.

**Art and Visuals Effects** - Art and visual effects of a game are vital. They immerse the player and create a unique experience that is fun and enjoyable. Improving the visuals of the game was possible given our experience, but due to time constraints and priority of other features it was not possible in the initial development period. Some visual improvements include: unit movement animations, unit battling animations and effects, unit death/lose animations and effects, tile effects and animations, improved UI responsiveness as well as visual appeal.

**Single Player AI** - With more time we would have liked to have built an AI the player can face in single player mode. The AI could be adjusted in difficulty increments such as easy, medium, and hard. The AI could be used to help the player learn the game as well as make our game appeal to people that prefer to play alone.

**Minor Bug Fixes and Tweaking** - Our game/application is functional but there still exists small bugs and areas that need further tweaking. Given more time we would have liked to have fixed the following: Some UI elements do not scale properly for all device sizes. Some UI elements are not perfectly synced in multiplayer. Tweaking gameplay balancing variables.


#### Minimum Functionality
- One (1) Resource: Money  (Completed)
- A tiled map with three segments, one for each player and a neutral zone in the middle (Completed)
- Single resource collection point in neutral zone (Completed)
- Two (2) buildings, one (1) which can defende a tile and one (1) that can increase the income of the tile (Completed)
- Two( 2) units: one (1) for building and gathering resources, and one (1) for fighting enemy units (Completed)

#### Expected Functionality
- One (1) trap a player can use defensively without the other player being aware (Completed)
- One (1) offensive action to capture an enemy tile (Completed)

#### Bonus Functionality
- Lobby based multiplayer (Completed)
- Basic sound effects (Completed)

## Sources
[1] "gamesplusjames", YouTube, 20-Jul-2016. [Online]. Available: https://www.youtube.com/watch?v=fQ2Dvj5-pfc.

[2] "Pan camera 2d by touch - Unity Answers". [Online]. Available: https://answers.unity.com/questions/517529/pan-camera-2d-by-touch.html.

[3] "Move Camera by drag - Unity Answers". [Online]. Available: https://answers.unity.com/questions/813224/move-camera-by-drag.html.

[4] "c# - Efficient way to remove ALL whitespace from String? - Stack Overflow". [Online]. Available: https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string.

[5] "Unity". [Online]. Available: https://unity3d.com/learn/tutorials/modules/intermediate/live-training-archive/modal-window.

[6] “Unity (game engine),” Wikipedia, 06-Apr-2018. [Online]. Available: https://en.wikipedia.org/wiki/Unity_(game_engine).

[7] “Background Music 찮 10,” 찮_ٶ֪, 24-Sep-2013. [Online]. Available: https://zhidao.baidu.com/question/133866833918388365.html?fr=ala&word=天天爱消除bgm. [Accessed: 08-Apr-2018].

[8] “How to hide a button programmatically?,” android - How to hide a button programmatically? - Stack Overflow. [Online]. Available: https://stackoverflow.com/questions/6173400/how-to-hide-a-button-programmatically. [Accessed: 08-Apr-2018].

[9] “How to hide a button programmatically?,” android - How to hide a button programmatically? - Stack Overflow. [Online]. Available: https://stackoverflow.com/questions/6173400/how-to-hide-a-button-programmatically. [Accessed: 08-Apr-2018].

[10] “How to enable multidexing with the new Android Multidex support library,” gradle - How to enable multidexing with the new Android Multidex support library - Stack Overflow. [Online]. Available: https://stackoverflow.com/questions/26609734/how-to-enable-multidexing-with-the-new-android-multidex-support-library. [Accessed: 08-Apr-2018].
