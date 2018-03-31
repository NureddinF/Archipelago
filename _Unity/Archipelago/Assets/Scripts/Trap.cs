using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Building {
	public HexGrid.TileType hexUnderTrap;

	public void setType(HexGrid.TileType type){
		hexUnderTrap = type;
	}

	public HexGrid.TileType getType(){
		return hexUnderTrap;
	}
}
