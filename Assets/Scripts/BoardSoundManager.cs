using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BoardSoundManager : MonoBehaviour {
	public AudioClip land;
	public AudioClip lineClear1;
	public AudioClip lineClear2;
	public AudioClip lineClear3;
	public AudioClip lineClear4;
	public AudioClip die;

	private AudioSource source;

	void Start () {
		source = GetComponent<AudioSource>();
	}

	public void PlaySound(BoardSounds sound_type) {
		AudioClip clip = null;
		if (sound_type == BoardSounds.land) {
			clip = land;
		}
		if (sound_type == BoardSounds.lineClear1) {
			clip = lineClear1;
		}
		if (sound_type == BoardSounds.lineClear2) {
			clip = lineClear2;
		}
		if (sound_type == BoardSounds.lineClear3) {
			clip = lineClear3;
		}
		if (sound_type == BoardSounds.lineClear4) {
			clip = lineClear4;
		}
		if (sound_type == BoardSounds.die) {
			clip = die;
		}
		
		
		if (clip != null)
			source.PlayOneShot(clip);
	}

}

public enum BoardSounds {
	land,
	lineClear1,
	lineClear2,
	lineClear3,
	lineClear4,
	die
}
