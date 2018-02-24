using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//reference: https://www.youtube.com/watch?v=BW7xXaFmSyY
public class Menu : MonoBehaviour {

	private const int menuIconWidth = 50;
	private const int menuIconHeight = 50;

	private List <MenuItem> menuItems = new List <MenuItem> ();
	public Player player;

	public Texture2D menuBackground;
	private Hex selectedHex;

	public void Update(){
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
			// Reference for sprite to texture conversion https://answers.unity.com/questions/651984/convert-sprite-image-to-texture.html

			icon.normal.background = menuItems [i].menuIcon;

			if(GUI.Button(new Rect(Screen.width - menuIconWidth, (menuIconHeight+padding)*i, menuIconWidth, menuIconHeight),"",icon)){
				Debug.Log ("Clicked " + menuItems [i]);
				if(menuItems[i].type == MenuItem.MenuItemType.UNIT){
					player.makeUnit (menuItems [i].objectPrefab, selectedHex.transform.position);
				} else if (menuItems[i].type == MenuItem.MenuItemType.BUILDING){
					player.upgradeTile (selectedHex, menuItems [i].objectPrefab);
				}
			}
		}
	}

	// Display the menu options for given hex
	public void updateMenu(Hex hex){
		selectedHex = hex;
		if (hex != null) {
			menuItems = hex.menuOptions;
		} else {
			menuItems = new List<MenuItem> ();
		}
	}
		
}
