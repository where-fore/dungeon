using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour {

	[SerializeField]
	private GameObject textObject;
	private Text timerTextComponent;

	private GameTurn gameTurnManager;



	// Use this for initialization
	void Awake () {
		timerTextComponent = textObject.GetComponent<Text>();
		gameTurnManager = GameObject.FindWithTag("Game Manager").GetComponent<GameTurn>();
	}
	
	// Update is called once per frame
	void Update () {

		timerTextComponent.text = gameTurnManager.turnsTaken.ToString();
		
	}
}
