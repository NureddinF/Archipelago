using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidWrapper : MonoBehaviour {

	void OnGUI() {
		if (GUI.Button (new Rect (10, 10, 150, 100), "Hello World")) {
			print ("You clicked the unity button!");
			AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

			currentActivity.Call("trigger");
		}

	}
}
