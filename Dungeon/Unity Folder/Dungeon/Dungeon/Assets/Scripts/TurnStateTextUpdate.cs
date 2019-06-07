using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnStateTextUpdate : MonoBehaviour {


	private Text turnStateText;

	private GameTurn gameTurnManager;

	// Use this for initialization
	void Awake () {
		
		// Get a hold of the game turn manager
		gameTurnManager = GameObject.FindWithTag("Game Manager").GetComponent<GameTurn>();


		turnStateText = GetComponent<Text>();
		turnStateText.enabled = false;


	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.L)) {

			turnStateText.enabled = !turnStateText.enabled;

		}
		
		if (gameTurnManager.playerTurn) {

			turnStateText.text = "Your Turn!";
			
		}
		else if (gameTurnManager.monsterTurn) {

			turnStateText.text = "Monster Turn!";
		}
		else {

			turnStateText.text = "Not your turn!";

		}

		turnStateText.text = turnStateText.text + "    Turns taken: " + gameTurnManager.turnsTaken;
		
	}
}
