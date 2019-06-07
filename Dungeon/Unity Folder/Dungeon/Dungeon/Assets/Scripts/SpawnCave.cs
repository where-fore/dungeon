using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCave : MonoBehaviour {

	[SerializeField]
	private GameObject player;
	[SerializeField]
	private GameObject monster;
	[SerializeField]
	private GameObject ladder;
	[SerializeField]
	private GameObject treasure;

	private int eastMovementBound = 12;
	private int westMovementBound = 1;
	private int northMovementBound = 5;
	private int southMovementBound = 1;

	private int tempX;
	private int tempY;
	private Vector2 tempLocation;

	private System.Random random;

	private List<Vector2> usedLocations;

	private bool needNewLocation = true;

	private int spawnedTreasure = 0;

	[SerializeField]
	public int treasureToSpawn = 5;

	private GameState gameStateManager;

	void Awake () {

		gameStateManager = GameObject.FindWithTag("Game Manager").GetComponent<GameState>();
		
		random = new System.Random();

		usedLocations = new List<Vector2>();

	}

	public void SpawnEverything () {

		SpawnLadder();

		while (spawnedTreasure < treasureToSpawn) {

			SpawnTreasure();
			spawnedTreasure = spawnedTreasure + 1;

		}

		SpawnMonster();

		SpawnPlayer();
		
	}

	private void SpawnLadder() {

		while (needNewLocation) {

			// Spawn only in perimeter by...
			// Rolling a d4...
			int d4 = random.Next(1, 5);
	
			// Chooses to spawn on a random location on the West, East, South, or North bounds of the map
			if (d4 == 1) {
				tempLocation = new Vector2(westMovementBound, RandomY());

			}

			else if (d4 == 2) {

				tempLocation = new Vector2(eastMovementBound, RandomY());

			}

			else if (d4 == 3) {

				tempLocation = new Vector2(RandomX(), southMovementBound);

			}

			else if (d4 == 4) {

				tempLocation = new Vector2(RandomX(), northMovementBound);

			}


			if (!usedLocations.Contains(tempLocation)) {

				needNewLocation = false;

			}
		}

		FinishSpawning(ladder);
		
	}

	private void SpawnMonster() {

		while (needNewLocation) {

			tempLocation = new Vector2(RandomX(), RandomY());

			// As long as the monster doesn't overlap a used tile, and doesn't rest on the east wall (monster x,y is the left/west square of the 1x2 model)
			if (!usedLocations.Contains(tempLocation) && tempLocation.x < (eastMovementBound - 1)) {

				needNewLocation = false;

			}

		}

		FinishSpawning(monster);


	}

	private void SpawnPlayer() {

		while (needNewLocation) {

			tempLocation = new Vector2(RandomX(), RandomY());

			if (!usedLocations.Contains(tempLocation)) {

				needNewLocation = false;

			}

		}

		FinishSpawning(player);

	}

	private void SpawnTreasure() {

		while (needNewLocation) {
			
			// Spawn on any location that is not the perimeter -> achieved by finding random x between west bound + 1, and east bound - 1 ; and east bound + 1 for random.next inclusivity.
			// Same for y.
			tempLocation = new Vector2(random.Next(westMovementBound + 1, (eastMovementBound - 1) + 1), random.Next(southMovementBound + 1, (northMovementBound - 1) + 1));

			if (!usedLocations.Contains(tempLocation)) {

				needNewLocation = false;

			}

		}

		FinishSpawning(treasure);

	}

	private void FinishSpawning(GameObject target) {

		// If the target is a prefab, not an instansted object...
		if (target.scene.rootCount == 0) {

			Instantiate(target, tempLocation, Quaternion.Euler(0, 0, 0));

		}
		// If the target is not a prefab, and is indeed an instanted object...
		else {

			target.transform.position = tempLocation;
			target.SetActive(true);

		}

		usedLocations.Add(tempLocation);
		
		if (target.CompareTag("Monster")) {

			usedLocations.Add(new Vector2(tempLocation.x + 1, tempLocation.y));
			gameStateManager.monsterSpawn = tempLocation;

		}

		if (target.CompareTag("Player")) {

			gameStateManager.playerSpawn = tempLocation;

		}

		tempLocation = Vector2.zero;

		needNewLocation = true;

	}

	private int RandomX() {

		return random.Next(westMovementBound, eastMovementBound + 1);

	}

	private int RandomY() {

		return random.Next(southMovementBound, northMovementBound + 1);

	}
}
