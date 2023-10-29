using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTurn : MonoBehaviour {

	public int turnsTaken = 0;

	public bool monsterTurn = false;
	public bool playerTurn = false;

	public bool waitingForMessage = false;

	public bool gameOngoing = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (!gameOngoing) {

			playerTurn = false;
			monsterTurn = false;
			
		}
		
	}

	public void PassTurn (GameObject passer) {

		if (passer.CompareTag("Player")) {

			StartCoroutine(PlayerPassTurn());
		
		}

		else if (passer.CompareTag("Monster")) {

			StartCoroutine(MonsterPassTurn());

		}


	}

	private IEnumerator PlayerPassTurn() {

		// End player turn
		playerTurn = false;

		//Clear text
		GameMessagesText.textComponent.text = "";
		GameMessagesTop.textComponent.text =  "";

		// Increment turns taken
		turnsTaken ++;

		// Wait for animations etc. to finish
		yield return new WaitForSeconds(0.3f);

		monsterTurn = true;


	}

	private IEnumerator MonsterPassTurn() {

		// End monster turn
		monsterTurn = false;

		// Wait for animations etc.
		yield return new WaitForSeconds(0.3f);

		playerTurn = true;

	}



}
