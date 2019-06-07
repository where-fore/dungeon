using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureUI : MonoBehaviour {

	[SerializeField]
	public GameObject textObject;
	private Text treasureTextComponent;

	private GameState gameStateManager;



	// Use this for initialization
	void Awake () {

		treasureTextComponent = textObject.GetComponent<Text>();

		gameStateManager = GameObject.FindWithTag("Game Manager").GetComponent<GameState>();

	}
	
	// Update is called once per frame
	void Update () {

		treasureTextComponent.text = "x " + gameStateManager.treasureCollected;
		
	}
}
