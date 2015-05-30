using UnityEngine;
using System.Collections;

public class PieceSpawner : MonoBehaviour {
	public GameObject fallingPiece;
	public int playerNumber;
	public DifficultyManager dm;
	public PieceList pieceList;

	// Use this for initialization
	void Start () {
		SpawnPiece ();
	}
	
	public void SpawnPieceWithDelay () {
		StartCoroutine ("SpawnPieceDelay");
	}
	
	private IEnumerator SpawnPieceDelay() {
		yield return new WaitForSeconds(dm.BetweenPieceDelay);
		SpawnPiece();
	}
	
	void SpawnPiece() {
		float z_angle = 90 * Mathf.Floor(4 * Random.value);
		GameObject block = Instantiate (fallingPiece, transform.position, Quaternion.Euler(0,0,z_angle)) as GameObject;		
		block.GetComponent<FallingPiece>().init (playerNumber, pieceList.getRandom());
	}
}
