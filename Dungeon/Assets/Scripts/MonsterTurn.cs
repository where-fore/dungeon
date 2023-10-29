using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGame;

public class MonsterTurn : MonoBehaviour {

	[SerializeField]
	private GameObject player;

	[SerializeField]
	private int northMovementBound = 5;

	[SerializeField]
	private int southMovementBound = 1;

	[SerializeField]
	private int eastMovementBound = 12;

	[SerializeField]
	private int westMovementBound = 1;

	private Vector3 moveTarget;

	private bool turnTaken = false;

	private GameTurn gameTurnManager;

	private GameState gameStateManager;

	private AudioControl audioManager;
	
	private Die die;

	private string awakenText = "You hear a low growl in the distance.";

	private string noMoveText = "The monster seems still.";

	private string moveSlowText = "You can hear movement in the distance.";

	private string moveFastText = "You hear growls and scraping claws on the walls around you - the monster is moving quickly.";

	private string silenceText = "There is an eerie calm in the dungeon.\nYou feel watched.";

	private string silenceRecurrentText = "\nYou feel watched.";

	private string eatenText = "You got gobbled!";

	private int firstMoveTurnNumber = 5;

	private int firstSilenceTurnNumber = 15;

	private int turnBetweenSilences = 10;

	// Silent turns previously taken, should always start at 0. This increments as silent turns are taken, and thus multiplies the turn count when a silence should occur.
	private int silentTurns = 0;

	// Chase turns remaining, usually should start at 0. Gets a value set if the monster hears a sound and decides to chase it.
	private int chaseTurns = 0;

	private Vector3 chaseTarget;

	// Use this for initialization
	private void Awake () {

		die = new DungeonGame.Die();

		gameTurnManager = GameObject.FindWithTag("Game Manager").GetComponent<GameTurn>();

		gameStateManager = GameObject.FindWithTag("Game Manager").GetComponent<GameState>();

		audioManager = GameObject.FindWithTag("Audio Manager").GetComponent<AudioControl>(); 

		// Account for the monster being 2 tiles long
		eastMovementBound = eastMovementBound - 1;

	}

	private void OnTriggerEnter2D (Collider2D other) {

		if (other.CompareTag("Player")) {

			gameTurnManager.gameOngoing = false;

			StartCoroutine(KillPlayer());

		}
	}
	
	// Update is called once per frame
	private void Update () {
		
		if (gameTurnManager.monsterTurn) {
			if (turnTaken != true) {
				TakeTurn();

			}
		}
		
	}

	private void TakeTurn() {	
		
		turnTaken = true;	
		moveTarget = DecideWhereToMove();
		MoveMonster(moveTarget);

	}

	private Vector3 DecideWhereToMove() {

		// Return doNotMove to tell the monster to move to it's current location; to not move.
		Vector3 doNotMove = transform.position;

		if (gameTurnManager.turnsTaken < firstMoveTurnNumber) {

			return doNotMove;

		}

		else if (gameTurnManager.turnsTaken == firstMoveTurnNumber) {

			GameMessagesText.textComponent.text = awakenText;
			return ChooseMoveClose();

		}

		// can change this arbitrary check into a function [return true if vvv ] 
		else if (gameTurnManager.turnsTaken > firstMoveTurnNumber && gameTurnManager.turnsTaken < firstSilenceTurnNumber) {

			// condense this into function RandomMoveBeforeFirstSilence()
			int result = die.d(6);

			if (result == 1 || result == 2 || result == 3) {

				GameMessagesText.textComponent.text = noMoveText;
				return doNotMove;

			}
			else if (result == 4 || result == 5) {

				GameMessagesText.textComponent.text = moveSlowText;
				return ChooseMoveClose();

			}
			else if (result == 6) {

				GameMessagesText.textComponent.text = moveFastText;
				return ChooseMoveMedium();

			}
			else {

				Debug.Log("Move Die result outside of programmed values");
				return doNotMove;

			}

		}
		// Condense to function and variable(turns of silence) that can be changed
		else if (gameTurnManager.turnsTaken == firstSilenceTurnNumber + (turnBetweenSilences * silentTurns)) {

			GameMessagesText.textComponent.text = silenceText;
			return doNotMove;

		}

		else if (gameTurnManager.turnsTaken == firstSilenceTurnNumber + (turnBetweenSilences * silentTurns) + 1) { 

			GameMessagesText.textComponent.text = silenceRecurrentText;
			silentTurns++;
			return doNotMove;			

		}

		else if (chaseTurns > 0) {

			GameMessagesText.textComponent.text = moveSlowText;
			chaseTurns --;
			return ChaseMove(2, chaseTarget);

		}

		else {

			int result = die.d(9);

			if (result == 1 || result == 2) {

				GameMessagesText.textComponent.text = noMoveText;
				return doNotMove;

			}
			else if (result == 3 || result == 4 || result == 5 || result == 6 || result == 7) {

				GameMessagesText.textComponent.text = moveSlowText;
				return ChooseMoveClose();

			}
			else if (result == 8) {

				GameMessagesText.textComponent.text = moveFastText;
				return ChaseMove(2, player.transform.position);

			}
			else if (result == 9) {

				GameMessagesText.textComponent.text = moveFastText;
				return ChooseMoveMedium();

			}
			else {

				Debug.Log("Move Die result outside of programmed values");
				return doNotMove;

			}


		}

		// Unnecessary failsafe currently, save for future potential use
		// Debug.Log("Movement failure, did not choose a pattern.");
		//return doNotMove;

	}

	public void HearSomething(Vector3 Sound) {
;
		chaseTurns = 2;
		chaseTarget = Sound;

	}

	private void MoveMonster(Vector3 target) {

		if (target == transform.position) {
			EndTurn();
		}

		else {

			while (transform.position != target) {

				// Match Y
				if (transform.position.y != target.y) {

					if (transform.position.y < target.y) {
						transform.position = transform.position + new Vector3(0, 1, 0);
					}

					else if (transform.position.y > target.y) {
						transform.position = transform.position - new Vector3(0, 1, 0);
					}

				}

				// Match X
				else if (transform.position.x != target.x) {

					if (transform.position.x < target.x) {
						transform.position = transform.position + new Vector3(1, 0, 0);
					}

					else if (transform.position.x > target.x) {
						transform.position = transform.position - new Vector3(1, 0, 0);
					}
				} 

			}

			// End Turn
			EndTurn();
					
		}
	}

	private void EndTurn() {

		gameTurnManager.PassTurn(gameObject);
		turnTaken = false;

	}

	private Vector3 ChooseMoveClose() {
		//  _ _ _
		// |o o o|
		// |o M o| 3 wide x 3 tall
		// |o_o_o|
		//

		// Generate possible destinations (based on x and y position of the monster relative to the walls, as defined by the Serialized fields north|east|south|west bound) for monster to pick from
		// Basically hardcoded physics detection
		var movePotentials = new List<Vector3>();

		int x = Mathf.RoundToInt(transform.position.x);
		int y = Mathf.RoundToInt(transform.position.y);

		if (x < eastMovementBound && y < northMovementBound) { 
			movePotentials.Add(new Vector3(x + 1, y + 1));
		}
		if (x < eastMovementBound) { 
			movePotentials.Add(new Vector3(x + 1, y));
		}
		if (x < eastMovementBound && y > southMovementBound) { 
			movePotentials.Add(new Vector3(x + 1, y - 1));
		}
		if (y > southMovementBound) { 
			movePotentials.Add(new Vector3(x, y - 1));
		}
		if (x > westMovementBound && y > southMovementBound) { 
			movePotentials.Add(new Vector3(x - 1, y - 1));
		}
		if (x > westMovementBound) { 
			movePotentials.Add(new Vector3(x - 1, y));
		}
		if (x > westMovementBound && y < northMovementBound) { 
			movePotentials.Add(new Vector3(x - 1, y + 1));
		}
		if (y < northMovementBound) { 
			movePotentials.Add(new Vector3(x, y + 1));
		}
		
		// Return a random destination from the potential moves calculated above
		System.Random random = new System.Random();
		return movePotentials[random.Next(0, movePotentials.Count)];

	}

	private Vector3 ChooseMoveMedium() {
		//  _________
		// |o o o o o| 5 wide x 3 tall
		// |o x M x o| Cannot move in "x"
		// |o_o_o_o_o|
		//

		// Generate possible destinations (based on x and y position of the monster relative to the walls, as defined by the Serialized fields north|east|south|west bound) for monster to pick from
		// Basically hardcoded physics detection
		var movePotentials = new List<Vector3>();

		int x = Mathf.RoundToInt(transform.position.x);
		int y = Mathf.RoundToInt(transform.position.y);

		if (y < northMovementBound) {
			movePotentials.Add(new Vector3(x, y + 1));
		}
		if (x < eastMovementBound && y < northMovementBound) { 
			movePotentials.Add(new Vector3(x + 1, y + 1));
		}
		if (x < eastMovementBound - 1 && y < northMovementBound) { 
			movePotentials.Add(new Vector3(x + 2, y + 1));
		}
		if (x < eastMovementBound - 1) { 
			movePotentials.Add(new Vector3(x + 2, y));
		}
		if (x < eastMovementBound - 1 && y > southMovementBound) { 
			movePotentials.Add(new Vector3(x + 2, y - 1));
		}
		if (x < eastMovementBound && y > southMovementBound) { 
			movePotentials.Add(new Vector3(x + 1, y - 1));
		}
		if (y > southMovementBound) { 
			movePotentials.Add(new Vector3(x, y - 1));
		}
		if (x > westMovementBound && y > southMovementBound) { 
			movePotentials.Add(new Vector3(x - 1, y - 1));
		}
		if (x > westMovementBound + 1 && y > southMovementBound) { 
			movePotentials.Add(new Vector3(x - 2, y - 1));
		}
		if (x > westMovementBound + 1) { 
			movePotentials.Add(new Vector3(x - 2, y));
		}
		if (x > westMovementBound + 1 && y < northMovementBound) { 
			movePotentials.Add(new Vector3(x - 2, y + 1));
		}
		if (x > westMovementBound && y < northMovementBound) { 
			movePotentials.Add(new Vector3(x - 1, y + 1));
		}


		// Return a random destination from the potential moves calculated above
		System.Random random = new System.Random();
		return movePotentials[random.Next(0, movePotentials.Count)];
	}

	private Vector3 ChooseMoveFar() {
	
		//    ___________
		//  _| o o o o o |_
		// | o o x x x o o | 
		// | o x x M x x o | 7 wide x 5 tall, corners cut 
		// |_o o x x x o o_| cannot move in "x"
		//   |_o_o_o_o_o_|
		//

		// Generate possible destinations (based on x and y position of the monster relative to the walls, as defined by the Serialized fields north|east|south|west bound) for monster to pick from
		// Basically hardcoded physics detection
		var movePotentials = new List<Vector3>();

		int x = Mathf.RoundToInt(transform.position.x);
		int y = Mathf.RoundToInt(transform.position.y);

		if (y < northMovementBound - 1) { 
			movePotentials.Add(new Vector3(x, y + 2));
		}
		if (x < eastMovementBound && y < northMovementBound - 1) { 
			movePotentials.Add(new Vector3(x + 1, y + 2));
		}
		if (x < eastMovementBound - 1 && y < northMovementBound - 1) { 
			movePotentials.Add(new Vector3(x + 2, y + 2));
		}
		if (x < eastMovementBound - 1 && y < northMovementBound) { 
			movePotentials.Add(new Vector3(x + 2, y + 1));
		}
		if (x < eastMovementBound - 2 && y < northMovementBound) { 
			movePotentials.Add(new Vector3(x + 3, y + 1));
		}
		if (x < eastMovementBound - 2) { 
			movePotentials.Add(new Vector3(x + 3, y));
		}
		if (x < eastMovementBound  - 2 && y > southMovementBound) { 
			movePotentials.Add(new Vector3(x + 3, y - 1));
		}
		if (x < eastMovementBound - 1 && y > southMovementBound) { 
			movePotentials.Add(new Vector3(x + 2, y - 1));
		}
		if (x < eastMovementBound - 1 && y > southMovementBound + 1) { 
			movePotentials.Add(new Vector3(x + 2, y - 2));
		}
		if (x < eastMovementBound && y > southMovementBound + 1) { 
			movePotentials.Add(new Vector3(x + 1, y - 2));
		}
		if (y > southMovementBound + 1) { 
			movePotentials.Add(new Vector3(x, y - 2));
		}
		if (x > westMovementBound && y > southMovementBound + 1) { 
			movePotentials.Add(new Vector3(x - 1, y - 2));
		}
		if (x > westMovementBound + 1 && y > southMovementBound + 1) { 
			movePotentials.Add(new Vector3(x - 2, y - 2));
		}
		if (x > westMovementBound + 1 && y > southMovementBound) { 
			movePotentials.Add(new Vector3(x - 2, y - 1));
		}
		if (x > westMovementBound + 2 && y > southMovementBound) { 
			movePotentials.Add(new Vector3(x - 3, y - 1));
		}
		if (x > westMovementBound + 2) { 
			movePotentials.Add(new Vector3(x - 3, y));
		}
		if (x > westMovementBound + 2 && y < northMovementBound) { 
			movePotentials.Add(new Vector3(x - 3, y + 1));
		}
		if (x > westMovementBound + 1 && y < northMovementBound) { 
			movePotentials.Add(new Vector3(x - 2, y + 1));
		}
		if (x > westMovementBound + 1 && y < northMovementBound - 1) { 
			movePotentials.Add(new Vector3(x - 2, y + 2));
		}
		if (x > westMovementBound && y < northMovementBound - 1) { 
			movePotentials.Add(new Vector3(x - 1, y + 2));
		}

		// Return a random destination from the potential moves calculated above
		System.Random random = new System.Random();
		return movePotentials[random.Next(0, movePotentials.Count)];
			
	}

	private Vector3 ChaseMove(int moves, Vector3 sound) {
		// Store the current position in a temporary position with which we will move with. This is so we can hypothetically move one tile at a time, to find the best destination.
		Vector3 temporaryTarget = transform.position;
		
		while (moves > 0) {

			// Check the distance between the sound and the current hypothetical position, and move in the appropriate direction.
			// Also decrement the amount of moves left to take.

			if ((sound.x - temporaryTarget.x) > 0.9f) { // It would make more sense if this line was [sound.x > temporaryTarget.x], but those values are floats and so cannot be directly compared.

				temporaryTarget = new Vector2(temporaryTarget.x + 1, temporaryTarget.y);
				moves --;

			}

			else if ((temporaryTarget.x - sound.x) > 0.9f) {

				temporaryTarget = new Vector2(temporaryTarget.x - 1, temporaryTarget.y);
				moves --;

			}

			else if ((sound.y - temporaryTarget.y) > 0.9f) {
				
				temporaryTarget = new Vector2(temporaryTarget.x, temporaryTarget.y + 1);
				moves --;

			}

			else if ((temporaryTarget.y - sound.y) > 0.9f) {

				temporaryTarget = new Vector2(temporaryTarget.x, temporaryTarget.y - 1);
				moves --;

			}
			else {
				// Failsafe to prevent infinite while loop crashes
				Debug.Log("Chase movement no movement.");
				return temporaryTarget;

			}

		}

		// Check collision
		if (temporaryTarget.x >= eastMovementBound) {

			temporaryTarget = new Vector2(eastMovementBound - 1, temporaryTarget.y);

		}

		if (temporaryTarget.x < westMovementBound) {

			temporaryTarget = new Vector2(westMovementBound, temporaryTarget.y);

		}

		if (temporaryTarget.y > northMovementBound) {

			temporaryTarget = new Vector2(temporaryTarget.x, northMovementBound);

		}

		if (temporaryTarget.x < southMovementBound) {

			temporaryTarget = new Vector2(temporaryTarget.x, southMovementBound);

		}
		return temporaryTarget;

	}

	private IEnumerator KillPlayer() {

		GameMessagesText.textComponent.text = eatenText;

		audioManager.PlayDeathStingAndTransitionToTitle();

		// For effect
		yield return new WaitForSeconds(1.8f);

		// Ends game and starts continue game process
		gameStateManager.EndGameDeath();

	}

}
