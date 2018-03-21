using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	// Won't be able to call android functions when testing in unity os turn on debug and set scene manually
	public bool isDebug = false;
	public int debugScene = 1;

	// Use this for initialization
	void Start () {
		
		int sceneToLoad = debugScene;
		if (!isDebug) {
			sceneToLoad = AndroidWrapper.getScene ();
		}
		SceneManager.LoadScene (sceneToLoad);
	}
}
