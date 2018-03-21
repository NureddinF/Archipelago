using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class ClientAttemptConnect : NetworkBehaviour {

	public Text enteredIpAddress;

	public void attemptConnection(){
		Debug.Log ("attempting connection");
		// Get the entered string
		string ipText = enteredIpAddress.text;
		//remove whitespace
		ipText = RemoveWhitespace(ipText);
		//split string into bytes
		string[] ipBytesString = ipText.Split ('.');

		//make sure the ip address if the correct format for IPv4 (X.X.X.X)
		if (ipBytesString.Length != 4) {
			Debug.Log ("bad length ('.' seperators)");
			return;
		}

		//make sure the ip addres is valid
		try{
			for (int i = 0; i < 4; i++) {
				Debug.Log("checking i=" + i + ", ip[i]=" + ipBytesString[i]);
				byte  b = Byte.Parse(ipBytesString[i]);
			}
		} catch (Exception e){
			Debug.Log ("bad format: " + e.ToString());
			return;
		}
		Debug.Log ("valid Ip address");
	}

	// https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string
	public static string RemoveWhitespace(string str) {
		return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
	}

}
