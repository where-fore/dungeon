using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour {
	[SerializeField]
	private AudioMixerSnapshot Game;
	[SerializeField]
	private AudioMixerSnapshot Death;
	[SerializeField]
	private AudioMixerSnapshot Escape;

	[SerializeField]
	private GameObject deathSource;
	private AudioSource deathSting;

	[SerializeField]
	private GameObject escapeSource;
 
	private void Start() {

		deathSting = deathSource.GetComponent<AudioSource>();

		TransitionToGame(0f);

	}

	public void TransitionToGame(float transitionTime = 5.5f) {

		Game.TransitionTo(transitionTime);

	}

	public void TransitionToDeath(float transitionTime = 2.0f) {

		Death.TransitionTo(transitionTime);

	}

	public void TransitionToEscape(float transitionTime = 0.0f) {

		Escape.TransitionTo(transitionTime);
		PlayEscapeMusic();

	}

	public void PlayDeathStingAndTransitionToTitle() {

		PlayDeathSting();
		TransitionToDeath();
		StartCoroutine("WaitThenTransititionToTitle");

	}

	private IEnumerator WaitThenTransititionToTitle() {

		float transitionAwayFromDeathTime = deathSting.clip.length - 5.5f;

		yield return new WaitForSeconds(transitionAwayFromDeathTime);
		TransitionToGame();

	}

	private void PlayDeathSting() {

		deathSting.Play();

	}

	private void PlayEscapeMusic() {

		escapeSource.GetComponent<MusicLoop>().PlayIntroAndLoop();

	}






}
