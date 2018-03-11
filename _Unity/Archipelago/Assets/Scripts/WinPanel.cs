using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

public class WinPanel : MonoBehaviour {

	public Text winstatement;
	public Button goBack;
	public GameObject windowObject;

	private static WinPanel winPanel;

	public static WinPanel instance(){
		if (!winPanel) {

			winPanel = FindObjectOfType(typeof (WinPanel)) as WinPanel;
			if (!winPanel) {
				Debug.LogError ("There needs to be aa active winPanel script on yout scene");
			}
		}

		return winPanel;
	}

	public void back(string statement, UnityAction backEvent){
		windowObject.SetActive (true);
		goBack.onClick.RemoveAllListeners ();
		goBack.onClick.AddListener (backEvent);

		this.winstatement.text = statement;
		goBack.gameObject.SetActive (true);

	}

}
