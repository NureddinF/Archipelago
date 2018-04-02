using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu: MonoBehaviour {

	public void exitGame(){
		AndroidWrapper.returnToAndroidMenu ();
	}

	public void toggleOptionsMenu(){
		gameObject.SetActive (!gameObject.activeInHierarchy);
	}
}
