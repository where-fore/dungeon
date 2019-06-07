using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureInteractivity : MonoBehaviour {

	[SerializeField]
	private Sprite closedChestSprite;
	[SerializeField]
	private Sprite openAndFullChestSprite;
	[SerializeField]
	private Sprite openAndEmptyChestSprite;

	private GameMessagesText gameMessagesText;

	private SpriteRenderer spriteRenderer;

	private bool chestLooted = false;

	private bool chestFound = false;

	private GameState gameStateManager;

	private MonsterTurn monster;

	private string foundTreasureText = "You found treasure!";

	private string lootTreasureText = "Hopefully you weren't too loud as you stuff the coins into your pack.";


	// Use this for initialization
	private void Awake () {
		
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = closedChestSprite;

		gameStateManager = GameObject.FindWithTag("Game Manager").GetComponent<GameState>();

		monster = GameObject.FindWithTag("Monster Parent").transform.Find("Monster").gameObject.GetComponent<MonsterTurn>();
		
	}
	
	// Update is called once per frame
	private void Update () {
		
	}

	private void OnTriggerEnter2D(Collider2D other) {

		if (other.CompareTag("Player") && !chestFound) {

			// Open chest: show open, unlooted sprite.
			spriteRenderer.sprite = openAndFullChestSprite;
			
			// Send message to GameMessagesUI
			GameMessagesTop.textComponent.text = foundTreasureText;

			chestFound = true;

		}

	}

	void OnTriggerStay2D (Collider2D other) {

		if (other.CompareTag("Player")) {

			if (!chestLooted) {

				if (Input.GetKeyDown(KeyCode.Return)) {

					LootChest();

				}
			}

		}

	}

	private void LootChest() {

		GameMessagesTop.textComponent.text = lootTreasureText;

		// Set chest state to looted, so it cannot be relooted.
		chestLooted = true;

		// Change sprite to the looted chest sprite.
		spriteRenderer.sprite = openAndEmptyChestSprite;

		// Update treasure count on TreasureUI
		gameStateManager.treasureCollected ++;

		//Alert monster to treasure stolen
		monster.HearSomething(transform.position);

	}

	public void ResetChest() {

		chestLooted = false;

		spriteRenderer.sprite = closedChestSprite;

	}
}
