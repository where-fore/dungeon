using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LadderInteractivity : MonoBehaviour {

	private GameState gameStateManager;

	private GameTurn gameTurnManager;

	private AudioControl audioManager;
	
	public bool usable = true;

	private string foundLadderText = "You found the ladder.";
	
	// private string foundLadderTextNotAllTreasure = "You found the ladder. You may leave now, or risk staying to collect more treasure.";

	// private string foundLadderTextAllTreasure = "You found the ladder. You may leave now, and return your bulging pack to town.";

	// private void GenerateLadderText() {

	// 	if (gameStateManager.treasureCollected < gameStateManager.maxTreasure) {

	// 		foundLadderText = foundLadderTextNotAllTreasure;
			
	// 	}
	// 	else if (gameStateManager.treasureCollected == gameStateManager.maxTreasure) {

	// 		foundLadderText = foundLadderTextAllTreasure;

	// 	}

	// }

	// Use this for initialization
	void Start () {

		gameStateManager = GameObject.FindWithTag("Game Manager").GetComponent<GameState>();

		gameTurnManager = GameObject.FindWithTag("Game Manager").GetComponent<GameTurn>();

		audioManager = GameObject.FindWithTag("Audio Manager").GetComponent<AudioControl>(); 
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D (Collider2D other) {

		if (other.CompareTag("Player")) {

			if (usable) {

				// GenerateLadderText();

				GameMessagesTop.textComponent.text = foundLadderText;

			}	

		}
		
		if (other.CompareTag("Monster")) {

			usable = false;

		}
	}

	void OnTriggerStay2D (Collider2D other) {

		if (other.CompareTag("Player")) {

			if (usable && gameTurnManager.gameOngoing) {

				if (Input.GetKeyDown(KeyCode.Return)) {

					Escape();

					gameTurnManager.gameOngoing = false;

					gameStateManager.EndGameEscape();

				}
			}

		}

	}

	private void Escape() {

		audioManager.TransitionToEscape();

		gameTurnManager.gameOngoing = false;

		gameStateManager.EndGameEscape();

	}

	void OnTriggerExit2D (Collider2D other) {

		if (other.CompareTag("Monster")) {

			usable = true;

		}

	}

}
