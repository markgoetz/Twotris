using UnityEngine;
using System.Collections;

public class ScoreKeeper : MonoBehaviour {
	public UIManager UIManager;

	private int score = 0;
	
	public void AddScore(int amount) {
		score += amount;
		UIManager.SendMessage("SetScore", score);
	}
}
