# Archipelago
Archipelago is a tile based mobile strategy game where players compete for territory with the goal of reaching a predetermined sum of money more quickly than their opponents.
Players can build worker units to capture new tiles and build new buildings, which help both to defend their territory as well as produce more money.
In addition players can also train warriors to battle the opponent head on to gain their territory and buildings.

^^^^^ One paragraph to describe your project. Your description should include the project concept andkey features implemented. ^^^^^

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

## Feature Section
###Local Storage
	- Remeber me function created on Login, users login credentials are saved and are loaded when user opens application.
###Touch
	- Pinch to zoom, drag to pan camera, and touch to select and object implemented.
###Sound
	- Background music available.
###Networking
	- Lobby based multiplayer created for users to play together online.

## Final Project Status
Write a description of the final status of the project. Did you achieve all your goals? What would be the next step for this project (if it were to continue)?


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

------ PRIOR README BELOW ------

CSCI4176 Group3
Minimum Functionality
At a minimum we shall deliver:
A tiled map with three segments, one for each player and a neutral zone in the middle
One (1) resource: money
Three (3) units: one (1) for building, one (1) for gathering resources, and one (1) for defending 
One (1) offensive action to capture an enemy tile
A single resource collection point in the neutral zone
Two (2) buildings, one(1) which can defend a tile and one (1) that can increase the income from a tile
One (1) trap a player can use defensively without the other player being aware of it
A stand in A.I. which acts as an opponent but performs no actions 
Basic Sound effects

Expected Functionality 
In addition to minimum functionality we expect to deliver:
Multiple resources with money being the primary resource and others being used to allow access to higher tier units
Multiple tiers of units that function as better versions of the basic units but require higher tier resources
Multiple resource collection points around the map of varied value.
Multiple offensive actions for the player to use against their opponent
Multiple traps a player can use defensively without the other player being aware of it

Bonus Expectation
Lobby based network multiplayer (no matchmaking or ranking)
Random map generation
Highly polished Visuals  
Team or Free-For-All game mode
More units, maps, tools, features, etc.
Player Progress Tracking
Local Multiplayer (2 Players 1 screen)
