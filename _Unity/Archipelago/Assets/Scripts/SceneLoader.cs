using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	//map strings for scene name to scene number.
	// Make sure this is matches build settings
	public static Dictionary<string, int> scenes = new Dictionary<string, int>{
		{ "init", 0 },
		{ "join", 1 },
		{ "host", 2 },
		{ "play", 3 }
	};

	// unity name of gameplay scene
	public const string gameplaySceneUnityName = "TestScene_48";

	// Won't be able to call android functions when testing in unity os turn on debug and set scene manually
	public bool isDebug = false;
	public int debugScene = 1;
	public bool isHost = false;

	// Use this for initialization
	void Start () {
		int sceneToLoad = debugScene;
		if (!isDebug) {
			string sceneName = AndroidWrapper.getAndroidSceneName ();
			if(sceneName.Equals("host")){
				isHost = true;
				InitialGameState.hostIpAddr = AndroidWrapper.getIpAddr();
			}
			
			sceneToLoad = scenes [sceneName];
		}

		InitialGameState.isHost = isHost;
		SceneManager.LoadScene (sceneToLoad);
	}
}
