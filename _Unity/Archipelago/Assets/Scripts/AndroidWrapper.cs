using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidWrapper : MonoBehaviour {

	public static void winAction(){
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

		currentActivity.Call("win");
	}
}
