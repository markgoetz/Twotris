using UnityEngine;
using System.Collections;

public class PieceSpawner : MonoBehaviour {
	public GameObject fallingPiece;
	public int playerNumber;
	public DifficultyManager dm;
	public PieceList pieceList;
	public GameObject nextPieceLocation;
	
	private GameObject next_piece;

	// Use this for initialization
	void Start () {
		SpawnFirstPiece();
	}
	
	void SpawnFirstPiece() {
		createNextPiece();
		SpawnPiece ();
	}
	
	public void SpawnPieceWithDelay () {
		StartCoroutine ("SpawnPieceDelay");
	}
	
	private IEnumerator SpawnPieceDelay() {
		yield return new WaitForSeconds(dm.BetweenPieceDelay);
		SpawnPiece();
	}
	
	void createNextPiece() {
		float z_angle = 90 * Mathf.Floor(4 * Random.value);
		GameObject piece = Instantiate (fallingPiece, nextPieceLocation.transform.position, Quaternion.Euler(0,0,z_angle)) as GameObject;		
		piece.GetComponent<FallingPiece>().init (this, pieceList.getRandom());
		next_piece = piece;
	}
	
	void startPieceFalling() {
		next_piece.transform.position = transform.position;
		next_piece.SendMessage("startFalling");
	}
	
	void SpawnPiece() {
		startPieceFalling();
		createNextPiece ();
	}
}
