using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLoop : MonoBehaviour {

	[SerializeField]
	private AudioSource introPiece;
	[SerializeField]
	private AudioSource loopPiece;


	public void PlayIntroAndLoop() {

		introPiece.Play();
		
		loopPiece.PlayScheduled(AudioSettings.dspTime + 2.286f);

	}
}
