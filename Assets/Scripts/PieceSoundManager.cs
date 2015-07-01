using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PieceSoundManager : MonoBehaviour {
	public AudioClip moveSound;
	public AudioClip rotateSound;
	public AudioClip fallSound;
	public AudioClip landSound;
	
	private AudioSource source;
	
	void Start() {
		source = GetComponent<AudioSource>();
	}
	
	public void PlaySound(PlayerSounds sound_type) {
		AudioClip clip = null;
		if (sound_type == PlayerSounds.Fall) {
			clip = fallSound;
		}
		if (sound_type == PlayerSounds.Move) {
			clip = moveSound;
		}
		if (sound_type == PlayerSounds.Rotate) {
			clip = rotateSound;
		}
		if (sound_type == PlayerSounds.Land) {
			clip = landSound;
		}
		
		if (clip != null)
			source.PlayOneShot(clip);
	}
}


public enum PlayerSounds {
	Move,
	Rotate,
	Fall,
	Land
}