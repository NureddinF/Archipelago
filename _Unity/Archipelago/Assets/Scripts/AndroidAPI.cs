using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class AndroidAPI : MonoBehaviour {

	[DllImport("androidplugin")]//determin the so file
	private static extern void calledFromUnity (string str);        

	public float x = 50;
	public float y = 30;
	public float width = 200;
	public float height = 100;
	public string text = "Hello world";



	void OnGUI (){
		if (GUI.Button (new Rect (x,y,width,height), text)) {
			calledFromUnity("Hello Android");
		}
	}

	//called from android
	void CalledFromAndroid(string str){
		text = str;
	}
}