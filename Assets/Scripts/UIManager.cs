using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	public Text scoreText;
	public Text linesText;
	public Text levelText;
	public Text gameOverText;
	
	private float _fadeTime = 3;
	
	public void SetScore(int score) {
		scoreText.text = "Score: " + score;
		scoreText.SendMessage("Tween");
	}
	
	public void SetLines(int lines) {
		linesText.text = "Lines: " + lines;
		linesText.SendMessage("Tween");
	}
	
	public void SetLevel(int level) {
		levelText.text = "Level: " + level;
		levelText.SendMessage("Tween");
	}
	
	public void Die() {
		StartCoroutine("gameOverFade");
	}
	
	private IEnumerator gameOverFade() {
		float time_elapsed = 0;
		
		while (time_elapsed < _fadeTime) {
			gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, time_elapsed / _fadeTime);
			yield return null;
			time_elapsed += Time.deltaTime;
		}
	}
}
