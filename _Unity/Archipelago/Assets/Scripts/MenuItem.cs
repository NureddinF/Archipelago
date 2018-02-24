using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script contains information about things needed to display something in the right menu
public class MenuItem : MonoBehaviour {

	public enum MenuItemType {UNIT, BUILDING};
	public MenuItemType type;

	// Texture for the icon to display in the menu
	public Texture2D menuIcon;

	// Prefab of the object to place into the world.
	// Not always used.
	public GameObject objectPrefab;

}
