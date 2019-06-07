using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineToggle : MonoBehaviour {

	private GameTurn gameTurnManager;

	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {

		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

		gameTurnManager = GameObject.FindWithTag("Game Manager").GetComponent<GameTurn>();
		
	}
	
	// Update is called once per frame
	void Update () {

		if (gameTurnManager.playerTurn) {

			spriteRenderer.enabled = true;

		}
		else {

			spriteRenderer.enabled = false;

		}
		
	}
}
