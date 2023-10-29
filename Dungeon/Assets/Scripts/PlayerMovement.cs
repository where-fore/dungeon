using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// if (target.y - transform.position.y > 1)
// change direction in MovePlayer, instead of update
//
// store transform.position
// compare transform.position AFTER move to stored number - and turn accordingly


public class PlayerMovement : MonoBehaviour {
	
	[SerializeField]
	private int northMovementBound = 5;

	[SerializeField]
	private int southMovementBound = 1;

	[SerializeField]
	private int eastMovementBound = 12;

	[SerializeField]
	private int westMovementBound = 1;
	private Vector3 moveTarget;
	private Rigidbody2D rigidBody2D;

	private GameTurn gameTurnManager;

	// Use this for initialization
	void Awake () {

		gameTurnManager = GameObject.FindWithTag("Game Manager").GetComponent<GameTurn>();
	
		rigidBody2D = gameObject.GetComponent<Rigidbody2D>();

	}
	
	// Update is called once per frame
	void Update () {

		if (gameTurnManager.playerTurn) {
		
			if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) && transform.position.y < northMovementBound) {

				transform.rotation = Quaternion.Euler(0, 0, 0);
				moveTarget = transform.position + Vector3.up;
				MovePlayer(moveTarget);

			}

			if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x > westMovementBound) {

				transform.rotation = Quaternion.Euler(0, 0, 90);
				moveTarget = transform.position + Vector3.left;
				MovePlayer(moveTarget);


			}

			if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) && transform.position.y > southMovementBound) {

				transform.rotation = Quaternion.Euler(0, 0, 180);
				moveTarget = transform.position + Vector3.down;
				MovePlayer(moveTarget);
		
			}

			if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x < eastMovementBound) {

				transform.rotation = Quaternion.Euler(0, 0, 270);
				moveTarget = transform.position + Vector3.right;
				MovePlayer(moveTarget);

			}

			if (Input.GetKeyDown(KeyCode.Space)) {

				moveTarget = transform.position;
				MovePlayer(moveTarget);

			}
		}
		
	}

	void MovePlayer(Vector3 target) {

		rigidBody2D.MovePosition(target);
		EndTurn();

	}

	void EndTurn() {

		gameTurnManager.PassTurn(gameObject);

	}
}
