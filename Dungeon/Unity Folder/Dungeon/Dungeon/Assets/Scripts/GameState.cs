using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameState : MonoBehaviour {
	[SerializeField]
	private GameObject fog;

	private Text narrativeText;

	[HideInInspector]
	public int treasureCollected;

	private GameTurn gameTurnManager;

	private GameObject mainMenu;

	[HideInInspector]
	public int maxTreasure = 0;

	[HideInInspector]
	public bool continueGame = false;

	[HideInInspector]
	public bool wonGame = false;

	[HideInInspector]
	public Vector2 playerSpawn;

	[SerializeField]
	private GameObject player;

	[HideInInspector]
	public Vector2 monsterSpawn;

	[SerializeField]
	private GameObject monster;

	private string escapeText = "You clamber up the ladder towards the promise of fresh air";

	private string quitGameText = "Press escape again to exit";


	void Awake () {

		// Get a hold of the game turn manager
		gameTurnManager = GameObject.FindWithTag("Game Manager").GetComponent<GameTurn>();

		mainMenu = GameObject.FindWithTag("Main Menu");
				
		//Activate the fog (it is only disabled for viewing pleasure during work)
		fog.SetActive(true);

	}

	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape) && gameTurnManager.gameOngoing) {

			if (GameMessagesTop.textComponent.text != quitGameText) {

				GameMessagesTop.textComponent.text = quitGameText;

			}
			else if (GameMessagesTop.textComponent.text == quitGameText) {

				// for editor
				// mainMenu.SetActive(true);
				// UnityEditor.EditorApplication.isPlaying = false;
		
				// for build
				Application.Quit();

			}



		}

	}

	public void StartGame() {

		if (!continueGame) {

			SetUpGame();

		}

		if (continueGame) {

			ResetGame();

		}

		gameTurnManager.gameOngoing = true;
					
		gameTurnManager.playerTurn = true;

	}

	private void ResetGame() {

		ResetChests();
		
		player.transform.position = playerSpawn;

		monster.transform.position = monsterSpawn;
		
		treasureCollected = 0;

		gameTurnManager.turnsTaken = 0;

		GameMessagesTop.textComponent.text = "";

		GameMessagesText.textComponent.text = "";

	}

	private void ResetChests() {

		foreach (GameObject chest in GameObject.FindGameObjectsWithTag("Treasure")) {

			chest.GetComponent<TreasureInteractivity>().ResetChest();

		}

	}

	private void SetUpGame() {

		gameObject.GetComponent<SpawnCave>().SpawnEverything();

		maxTreasure = gameObject.GetComponent<SpawnCave>().treasureToSpawn;

	}

	public void EndGameDeath() {

		continueGame = true;

		mainMenu.SetActive(true);

	}

	public void EndGameEscape() {

		StartCoroutine(EscapeSequence());
						
	}

	private IEnumerator EscapeSequence() {

		GameMessagesText.textComponent.text = escapeText;

		wonGame = true;

		yield return new WaitForSeconds(0.5f);

		yield return waitForKeyPress(KeyCode.Return);

		mainMenu.SetActive(true);

	}	

	private IEnumerator waitForKeyPress(KeyCode key) {

		bool done = false;

		while (!done) {

			if (Input.GetKeyDown(key)) {

				done = true;

			}
			
			yield return null;

		}

	}
}
