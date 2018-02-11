using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class acquireTerritory : MonoBehaviour {

	private SpriteRenderer spriteRend;
	public Sprite defaultTile;
	public Sprite ownedTile;
	public Sprite enemyTile;
	public int owned;

	// Use this for initialization
	void Start () {
		spriteRend = GetComponent<SpriteRenderer> ();	
		if (spriteRend.sprite == null) {
			spriteRend.sprite = defaultTile;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) // If the space bar is pushed down
		{
			spriteRend.sprite = enemyTile;
		}
		if (Input.GetMouseButtonDown(0)) {
			spriteRend.sprite = ownedTile;
		}
		if (Input.GetKeyDown (KeyCode.A)) // If the space bar is pushed down
		{
			spriteRend.sprite = defaultTile;
		}
	}
		
}
