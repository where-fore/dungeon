using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInteractivity : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	[HideInInspector]
	public List<GameObject> adjacentWalls;

	[HideInInspector]
	public bool revealed;

	// Use this for initialization
	void Awake () {

		spriteRenderer = GetComponent<SpriteRenderer>();

		FindAdjacentWalls();

	}

	private void FindAdjacentWalls() {

		Collider2D[] allCollInOneByOne = Physics2D.OverlapAreaAll(new Vector2(gameObject.transform.position.x - 1, transform.position.y - 1), new Vector2(gameObject.transform.position.x + 1, transform.position.y + 1));

		foreach (Collider2D collider in allCollInOneByOne) {

			adjacentWalls.Add(collider.gameObject);

		}

	}

	void Update () {

		if (!revealed) {

			if (CheckAdjacentWalls()) {

				RevealWall();

			}

		}

	}

	private bool CheckAdjacentWalls() {
		// Returns true if there is both: 1) a revealed wall that is on a different x level than gameobject, and 2) a revealed wall that is on a different y level than gameobject
		// For this project, it is to return true for corner walls if their two adjacents are revealed.
		
		bool verticallyDifferent = false;
		bool horizontallyDifferent = false;

		foreach (GameObject wall in adjacentWalls) {

			if (wall.GetComponent<WallInteractivity>().revealed) {

				if (wall.transform.position.y != transform.position.y) {

					verticallyDifferent = true;

				}

				if (wall.transform.position.x != transform.position.x) {

					horizontallyDifferent = true;

				}

			}

		}

		if (horizontallyDifferent && verticallyDifferent) {

			return true;

		}
		else return false;


	}
	
	void OnCollisionEnter2D(Collision2D coll) {

		if (coll.gameObject.CompareTag("Player")) {

			RevealWall();

		}

	}

	private void RevealWall() {

		revealed = true;

		// Sets above fog.
		spriteRenderer.sortingLayerName = "Above Fog";
		
		// Removes fog under wall when wall is uncovered.
		var allCollOnTile = Physics2D.OverlapAreaAll(new Vector2(gameObject.transform.position.x - 0.5f, gameObject.transform.position.y - 0.5f), new Vector2(gameObject.transform.position.x + 0.5f, gameObject.transform.position.y + 0.5f));

		foreach (var thing in allCollOnTile) {

			if (thing.gameObject.CompareTag("Fog")) {

				thing.gameObject.SetActive(false);

			}

		}

	}

}
