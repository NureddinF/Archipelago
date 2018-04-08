using System.Collections;
using System.Collections.Generic;

// This class keeps track of gamestate as players connect
public class InitialGameState {

	public static bool isHost = false; //default to client

	public static string HostIpAddr = "127.0.0.1"; //default to localhost
	public static string ClientIpAddr = "127.0.0.1"; //default to localhost

	public static string username = "Player"; //local player's username

	public static Player.PlayerId nextPid = Player.PlayerId.P1; //used by server to assign player

	public static Player.PlayerId clientPid = Player.PlayerId.NEUTRAL; //client player identification
}

