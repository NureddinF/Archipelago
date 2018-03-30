using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidWrapper {

	public static void winAction(){
		AndroidJavaObject currentActivity = getUnityActivity();

		currentActivity.Call("win");
	}

	public static string getAndroidStartCommand(){

		AndroidJavaObject currentActivity = getUnityActivity();

		return currentActivity.Call<string>("getScene");
	}

	public static string getIpAddr(){
		AndroidJavaObject currentActivity = getUnityActivity();

		return currentActivity.Call<string>("getIpAddr");
	}

	public static string getUsername(){
		AndroidJavaObject currentActivity = getUnityActivity();

		return currentActivity.Call<string>("getUsername");
	}

	public static void returnToAndroidMenu(){
		AndroidJavaObject currentActivity = getUnityActivity();
		currentActivity.Call("exit");
	}

	private static AndroidJavaObject getUnityActivity(){
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

		return currentActivity;
	}

}
