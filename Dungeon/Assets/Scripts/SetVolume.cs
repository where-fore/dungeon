using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour {

	public AudioMixer mixer;

	private void Start () {

		if (PlayerPrefs.HasKey("Volume")) {

			gameObject.GetComponent<Slider>().value = PlayerPrefs.GetFloat("Volume");

		}

		// How to set volume with script:
		//
		// mixer.SetFloat("MasterVolume", -20f);
		//
		// or:
		// gameObject.GetComponent<Slider>().value = 0.5f;

	}

	public void SetLevel (float sliderValue) {

		mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);

	}

	public void SaveLevel () {

		PlayerPrefs.SetFloat("Volume", gameObject.GetComponent<Slider>().value);

	}

}
