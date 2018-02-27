using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//reference: https://www.youtube.com/watch?v=BW7xXaFmSyY
public class Menu : MonoBehaviour {

	// Dimensions of icons in the menu
	private const int menuIconWidth = 50;
	private const int menuIconHeight = 50;
	// The background image for the menu
	public Texture2D menuBackground;

	// Menu options to display when player clicks on the hex
	private List <MenuItem> menuItems = new List <MenuItem> ();
	// The hex that the player clicked on
	private Hex selectedHex;
	// Player who is interacting with the menu
	public Player player;


	public void Update(){
		// make sure menu stays up to date if hex menu options change (e.g. if tile is upgraded)
		updateMenu (selectedHex);
	}

	// Create menu on right side of screen
	public void OnGUI(){
		//Load background textrue
		GUIStyle container = new GUIStyle ();
		container.normal.background = menuBackground;
		GUI.Box (new Rect (Screen.width - menuIconWidth, 0, menuIconWidth, Screen.height), "", container);

		//Load the specific buttons
		int padding = 5;
		for(int i=0; i < menuItems.Count; i++){
			GUIStyle icon = new GUIStyle ();
			icon.normal.background = menuItems [i].menuIcon;

			if(GUI.Button(new Rect(Screen.width - menuIconWidth, (menuIconHeight+padding)*i, menuIconWidth, menuIconHeight),"",icon)){
				//Button was clicked
				if(menuItems[i].type == MenuItem.MenuItemType.UNIT){
					// player clicked on a unit button
					player.makeUnit (menuItems [i].objectPrefab, selectedHex.transform.position);
				} else if (menuItems[i].type == MenuItem.MenuItemType.BUILDING){
					// player clicked on a building button
					player.upgradeTileToBuilding (selectedHex, menuItems [i].objectPrefab);
				}
			}
		}
	}

	// Display the menu options for given hex
	public void updateMenu(Hex hex){
		selectedHex = hex;
		if (hex != null) {
			// Display menu options for the hex
			menuItems = hex.menuOptions;
		} else {
			// Clear menu options
			menuItems = new List<MenuItem> ();
		}
	}
		
}
