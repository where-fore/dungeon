using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartingScreen : MonoBehaviour {

	private IEnumerator coroutine;
	[SerializeField]
	private GameObject slider;

	[SerializeField]
	private GameObject textObject;

	private Text startingTextComponent;

	private bool narrationHandled;

	private GameState gameStateManager;

	private GameTurn gameTurnManager;

	private AudioControl gameAudioManager;

	private string[] openingNarration = {
		"You awaken on another hot day, truly ready to sell some crisp apples in the square.", //1
		"But where's the fun in that?\nYou set off adventuring, on a quest to find great loot.", //2
	};

	private List<string> continueNarration;

	private void CreateContinueNarration() {
		continueNarration = new List<string>();

		continueNarration.Add("Your spelunk resulted in death! You survived " + gameTurnManager.turnsTaken + " turns."); //1
		continueNarration.Add("You awaken on another hot day, truly ready to sell some juicy plums in the square."); //2
		continueNarration.Add("But where's the fun in that?\n With a well-worn map guiding your path, you set off on the quest for great loot. "); //3
	}

	private List<string> escapeNarration;

	private void CreateEscapeNarration() {
		escapeNarration = new List<string>();

		if (gameStateManager.continueGame) {
			
			if (gameStateManager.treasureCollected == 0) {			
				escapeNarration.Add("Congratulations! You escaped with your life. Spelunking is dangerous, maybe it's time to go back to selling plums."); //1

			}
			else if (gameStateManager.treasureCollected == 1) {
				escapeNarration.Add("Congratulations! You escaped with a piece of loot.\n\nSpelunking may be dangerous, but you turned a pretty penny!"); //1
				
			}
			else if (gameStateManager.treasureCollected > 1) {
				escapeNarration.Add("Congratulations! You escaped with " + gameStateManager.treasureCollected + " pieces of loot.\n\nSpelunking may be dangerous, but with these spoils you'll never have to leave your home again!"); //1
				
			}

		}
		else {
		
			if (gameStateManager.treasureCollected == 0) {			
				escapeNarration.Add("Congratulations! You escaped with your life. Spelunking is dangerous, maybe it's time to go back to selling apples."); //1

			}
			else if (gameStateManager.treasureCollected == 1) {
				escapeNarration.Add("Congratulations! You escaped with a piece of loot.\n\nSpelunking may be dangerous, but you turned a pretty penny!"); //1

			}
			else if (gameStateManager.treasureCollected > 1) {
				escapeNarration.Add("Congratulations! You escaped with " + gameStateManager.treasureCollected + " pieces of loot.\n\nSpelunking may be dangerous, but with these spoils you'll never have to leave your home again!"); //1
				
			}
		}

	}

	// Use this for initialization
	void Awake() {

		startingTextComponent = textObject.GetComponent<Text>();
		gameStateManager = GameObject.FindWithTag("Game Manager").GetComponent<GameState>();
		gameTurnManager = GameObject.FindWithTag("Game Manager").GetComponent<GameTurn>();
		gameAudioManager = GameObject.FindWithTag("Audio Manager").GetComponent<AudioControl>();
		
	}

	private void Update() {

		// This uses keyUp so I can use keyDown during regular gameplay. If this was keyDown, then this would trigger the exit during gameplay as well.
		if (Input.GetKeyUp(KeyCode.Escape)) {

			gameAudioManager.TransitionToGame(2);
			EndNarration();

		}

	}

	private void OnEnable() {

		StartCoroutine(Narration());

	}

	private IEnumerator Narration() {
		
		var text = CreateNarration();

		int lineNumber = 0;
		foreach (string line in text) {

			lineNumber ++;

			// UpdateGameAudio(lineNumber);

			startingTextComponent.text = line;
			yield return waitForKeyPress(new KeyCode[2] {KeyCode.Return, KeyCode.Space});

		}

		EndNarration();

	}


	private List<string> CreateNarration() {

		var text = new List<string>();

		if (gameStateManager.wonGame) {

			CreateEscapeNarration();

			text.AddRange(escapeNarration);

		}
		else if (gameStateManager.continueGame) {

			CreateContinueNarration();

			text.AddRange(continueNarration);

		}
		else {

			text.AddRange(openingNarration);

		}

		return text;

	}
	
	private IEnumerator waitForKeyPress(KeyCode[] keyArray) {

		bool done = false;

		while (!done) {

			foreach (KeyCode key in keyArray) {

				if (Input.GetKeyDown(key)) {

					done = true;

				}
			}
			
			yield return null;

		}

	}

	private void EndNarration() {

		slider.GetComponent<SetVolume>().SaveLevel();


		if (gameStateManager.wonGame) {

			QuitGame();

		}

		else {

			startingTextComponent.text = "";

			gameStateManager.StartGame();

			gameObject.SetActive(false);


		}


	}

	private void QuitGame() {

		// Used in editor only
		// UnityEditor.EditorApplication.isPlaying = false;
		
		Application.Quit();

	}

}
