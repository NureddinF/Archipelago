using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidWrapper : MonoBehaviour {

	public static int intitalScene = 0;

	private static Dictionary<string, int> scenes = new Dictionary<string, int>{
		{ "play", 1 },
		{ "host", 2 },
		{ "join", 3 }
	};

	public static void winAction(){
		AndroidJavaObject currentActivity = getUnityActivity();

		currentActivity.Call("win");
	}

	public static int getScene(){

		AndroidJavaObject currentActivity = getUnityActivity();

		string scene = currentActivity.Call<string>("getScene");
		intitalScene = scenes [scene];
		return intitalScene;
	}


	private static AndroidJavaObject getUnityActivity(){
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

		return currentActivity;
	}
}
