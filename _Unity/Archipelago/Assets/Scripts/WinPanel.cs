using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

public class WinPanel : MonoBehaviour {
	//https://unity3d.com/learn/tutorials/modules/intermediate/live-training-archive/modal-window

	public Text winstatement; //statment displayed in pop-up box
	public Button goBack; //back button
	public GameObject windowObject;

	private static WinPanel winPanel;

	public static WinPanel instance(){ //finds the instance of the win panel
		if (!winPanel) {

			winPanel = FindObjectOfType(typeof (WinPanel)) as WinPanel;
			if (!winPanel) {
				Debug.LogError ("There needs to be aa active winPanel script on yout scene");
			}
		}

		return winPanel;
	}

	public void back(string statement, UnityAction backEvent){ //takes in the statement to be displayed, and the backbutton action
		windowObject.SetActive (true);  //sets window to true
		goBack.onClick.RemoveAllListeners (); //removes any previouse listeners
		goBack.onClick.AddListener (backEvent); //creates a listener to the back button

		this.winstatement.text = statement;  //sets the statement to the statment to bo displayes
		goBack.gameObject.SetActive (true); //makes the backbutton active

	}

}
