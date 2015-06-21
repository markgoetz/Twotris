using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public PieceSpawner[] spawners;
	public int scorePerBlock = 10;
	public int[] scorePerClearedLines = {0,100,300,600,1000};
	public Board board;
	public UIManager uiManager;
	public DifficultyManager dm;
	
	private int score = 0;
	private int lines = 0;
	
	// What do you do if a block lands?
	public void PieceLanded(int player_number) {
		score += scorePerBlock;
		
		// check if any lines have been cleared (from top to bottom), and update the score
		int lines_just_cleared = board.processClears();
		if (lines_just_cleared > 0) {
			lines += lines_just_cleared;
			score += scorePerClearedLines[lines_just_cleared];
		}
		
		dm.Lines = lines;
		
		if (board.GameOver)
			gameOver ();
		
		uiManager.updateText(score, dm.Lines, dm.Level);
		
		// prepare the next block
		spawners[player_number - 1].SpawnPieceWithDelay();
	}
	
	public int Score { get { return score; } }
	
	private void gameOver() {
		uiManager.gameOver();
		
		board.SendMessage("DieEffect");
		
		foreach (PieceSpawner spawner in spawners) {
			Destroy(spawner.gameObject);
		}
		
		GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
		foreach (GameObject piece in pieces) {
			Destroy (piece);
		}		
	}
	
	void Start() {
		dm.Lines = 0;
	}
}
