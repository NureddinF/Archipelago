using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidWrapper {

	public static void winAction(){
		AndroidJavaObject currentActivity = getUnityActivity();

		currentActivity.Call("win");
	}

	public static string getAndroidSceneName(){

		AndroidJavaObject currentActivity = getUnityActivity();

		return currentActivity.Call<string>("getScene");
	}


	private static AndroidJavaObject getUnityActivity(){
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

		return currentActivity;
	}
}
