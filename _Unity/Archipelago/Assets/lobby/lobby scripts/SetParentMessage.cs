using System;
using UnityEngine;
using UnityEngine.Networking;

public class SetParentMessage : MessageBase{

	private enum Type
	{
		SetParent = MsgType.Highest + 1
	}

	public const short SetParent = (short)Type.SetParent;

	public NetworkInstanceId netId;
	public NetworkInstanceId parentNetId;

	public SetParentMessage(){
		
	}

	public SetParentMessage(GameObject gameObject, GameObject parent){
		netId = gameObject.GetComponent<NetworkIdentity>().netId;
		parentNetId = parent.GetComponent<NetworkIdentity>().netId;
	}
}

